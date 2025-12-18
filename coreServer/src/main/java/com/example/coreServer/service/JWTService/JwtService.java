package com.example.coreServer.service.JWTService;

import com.example.coreServer.model.User;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;
import java.time.Duration;
import java.time.Instant;
import java.util.Date;
import java.util.UUID;

@Service
public class JwtService {

    private final SecretKey key;
    private final String issuer;
    private final Duration accessTtl;
    private final Duration clockSkew;

    public JwtService(
            @Value("${app.security.jwt.hmac-secret}") String secret,
            @Value("${app.security.jwt.issuer}") String issuer,
            @Value("${app.security.jwt.access-ttl-minutes}") long ttlMinutes,
            @Value("${app.security.jwt.clock-skew-seconds:60}") long clockSkewSeconds
    ) {
        this.key = Keys.hmacShaKeyFor(secret.getBytes(StandardCharsets.UTF_8));
        this.issuer = issuer;
        this.accessTtl = Duration.ofMinutes(ttlMinutes);
        this.clockSkew = Duration.ofSeconds(clockSkewSeconds);
    }

    public String issueAccessToken(User user) {
        Instant now = Instant.now();
        Instant exp = now.plus(accessTtl);

        return Jwts.builder()
                .issuer(issuer)
                .subject(String.valueOf(user.getId()))
                .claim("username", user.getUsername())
                .issuedAt(Date.from(now))
                .expiration(Date.from(exp))
                .id(UUID.randomUUID().toString())
                .signWith(key)
                .compact();
    }

    public JwtPayload parseAndValidate(String token) {
        var claims = Jwts.parser()
                .verifyWith(key)
                .requireIssuer(issuer)
                .clockSkewSeconds(clockSkew.toSeconds())
                .build()
                .parseSignedClaims(token)
                .getPayload();

        Long userId = Long.valueOf(claims.getSubject());
        String username = claims.get("username", String.class);
        return new JwtPayload(userId, username);
    }

    public record JwtPayload(Long userId, String username) {}
}
