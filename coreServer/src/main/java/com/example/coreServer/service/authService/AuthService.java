package com.example.coreServer.service.authService;

import com.example.coreServer.dto.authDto.LoginRequest;
import com.example.coreServer.dto.authDto.RegisterRequest;
import com.example.coreServer.model.RefreshToken;
import com.example.coreServer.model.User;
import com.example.coreServer.repository.RefreshTokenRepository;
import com.example.coreServer.repository.UserRepository;
import com.example.coreServer.service.JWTService.JwtService;
import com.example.coreServer.security.RefreshTokenCodec;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.time.temporal.ChronoUnit;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final UserRepository userRepository;
    private final RefreshTokenRepository refreshTokenRepository;
    private final JwtService jwtService;
    private final RefreshTokenCodec refreshTokenCodec;

    private final BCryptPasswordEncoder encoder = new BCryptPasswordEncoder();

    @Value("${app.security.refresh.ttl-days}")
    private int refreshTtlDays;

    public void register(RegisterRequest req) {
        String username = req.username().trim();
        if (username.isBlank() || req.password() == null || req.password().length() < 6) {
            throw new IllegalArgumentException("Invalid username/password");
        }
        if (userRepository.existsByUsername(username)) {
            throw new IllegalStateException("Username already taken");
        }

        User user = User.builder()
                .username(username)
                .passwordHash(encoder.encode(req.password()))
                .displayName(req.displayName())
                .deleted(false)
                .build();

        userRepository.save(user);
    }

    public LoginResult login(LoginRequest req, String userAgent, String ip) {
        User user = userRepository.findByUsername(req.username())
                .filter(u -> !u.isDeleted())
                .orElseThrow(() -> new IllegalArgumentException("Bad credentials"));

        if (!encoder.matches(req.password(), user.getPasswordHash())) {
            throw new IllegalArgumentException("Bad credentials");
        }

        String access = jwtService.issueAccessToken(user);

        String refreshRaw = refreshTokenCodec.generateRaw();
        String refreshHash = refreshTokenCodec.hash(refreshRaw);

        RefreshToken rt = RefreshToken.builder()
                .user(user)
                .tokenHash(refreshHash)
                .expiresAt(Instant.now().plus(refreshTtlDays, ChronoUnit.DAYS))
                .userAgent(userAgent)
                .build();

        refreshTokenRepository.save(rt);

        return new LoginResult(access, refreshRaw);
    }

    public LoginResult refresh(String refreshRaw, String userAgent, String ip) {
        if (refreshRaw == null || refreshRaw.isBlank()) {
            throw new IllegalArgumentException("No refresh token");
        }

        String hash = refreshTokenCodec.hash(refreshRaw);
        RefreshToken existing = refreshTokenRepository.findByTokenHash(hash)
                .orElseThrow(() -> new IllegalArgumentException("Invalid refresh token"));

        Instant now = Instant.now();
        if (!existing.isActive(now)) {
            throw new IllegalArgumentException("Refresh token expired/revoked");
        }

        existing.setRevokedAt(now);

        User user = existing.getUser();
        String newAccess = jwtService.issueAccessToken(user);

        String newRefreshRaw = refreshTokenCodec.generateRaw();
        String newRefreshHash = refreshTokenCodec.hash(newRefreshRaw);

        RefreshToken next = RefreshToken.builder()
                .user(user)
                .tokenHash(newRefreshHash)
                .expiresAt(now.plus(refreshTtlDays, ChronoUnit.DAYS))
                .userAgent(userAgent)
                .build();

        refreshTokenRepository.save(next);
        existing.setReplacedBy(next);
        refreshTokenRepository.save(existing);

        return new LoginResult(newAccess, newRefreshRaw);
    }

    public void logout(String refreshRaw) {
        if (refreshRaw == null || refreshRaw.isBlank()) return;

        String hash = refreshTokenCodec.hash(refreshRaw);
        refreshTokenRepository.findByTokenHash(hash).ifPresent(rt -> {
            rt.setRevokedAt(Instant.now());
            refreshTokenRepository.save(rt);
        });
    }

    public record LoginResult(String accessToken, String refreshTokenRaw) {}
}
