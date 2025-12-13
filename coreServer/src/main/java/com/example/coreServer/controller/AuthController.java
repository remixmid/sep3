package com.example.coreServer.controller;

import com.example.coreServer.dto.authDto.AuthResponse;
import com.example.coreServer.dto.authDto.LoginRequest;
import com.example.coreServer.dto.authDto.MeResponse;
import com.example.coreServer.dto.authDto.RegisterRequest;
import com.example.coreServer.model.User;
import com.example.coreServer.repository.UserRepository;
import com.example.coreServer.service.authService.AuthService;
import com.example.coreServer.web.RefreshCookieFactory;
import jakarta.servlet.http.HttpServletRequest;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/auth")
@RequiredArgsConstructor
public class AuthController {

    private final AuthService authService;
    private final RefreshCookieFactory cookieFactory;
    private final UserRepository userRepository;

    /**
     * Register a new user.
     * POST /auth/register
     * Body: RegisterRequest (username, password, displayName, ...)
     */
    @PostMapping("/register")
    public ResponseEntity<Void> register(@RequestBody RegisterRequest req) {
        authService.register(req);
        return ResponseEntity.ok().build();
    }

    /**
     * Login using username + password.
     * POST /auth/login
     * Body: LoginRequest (username, password)
     * Returns:
     * - 200 OK + AuthResponse { accessToken }
     * - 4xx if credentials are invalid.
     */
    @PostMapping("/login")
    public ResponseEntity<AuthResponse> login(@RequestBody LoginRequest req, HttpServletRequest http) {
        var res = authService.login(req, http.getHeader("User-Agent"), http.getRemoteAddr());

        return ResponseEntity.ok()
                .header(HttpHeaders.SET_COOKIE, cookieFactory.create(res.refreshTokenRaw()).toString())
                .body(new AuthResponse(res.accessToken()));
    }

    /**
     * Refresh access token using refresh token from cookie.
     * POST /auth/refresh
     * Cookie:
     * - refresh_token (HttpOnly) is read from incoming request cookies.
     * Returns:
     * - 200 OK + AuthResponse { accessToken }
     * - 401/4xx if refresh token is missing/invalid/expired/revoked.
     */
    @PostMapping("/refresh")
    public ResponseEntity<AuthResponse> refresh(
            @CookieValue(name = "${app.security.refresh.cookie-name}", required = false) String refreshToken,
            HttpServletRequest http
    ) {
        var res = authService.refresh(refreshToken, http.getHeader("User-Agent"), http.getRemoteAddr());

        return ResponseEntity.ok()
                .header(HttpHeaders.SET_COOKIE, cookieFactory.create(res.refreshTokenRaw()).toString())
                .body(new AuthResponse(res.accessToken()));
    }

    /**
     * Logout (invalidate current refresh token session).
     * POST /auth/logout
     * Cookie:
     * - refresh_token (HttpOnly)
     * Returns:
     * - 200 OK always (even if cookie was missing), by design.
     */
    @PostMapping("/logout")
    public ResponseEntity<Void> logout(
            @CookieValue(name = "${app.security.refresh.cookie-name}", required = false) String refreshToken
    ) {
        authService.logout(refreshToken);
        return ResponseEntity.ok()
                .header(HttpHeaders.SET_COOKIE, cookieFactory.clear().toString())
                .build();
    }

    /**
     * Get current authenticated user profile.
     * GET /auth/me
     * Auth:
     * - Requires Authorization: Bearer <accessToken>
     * Returns:
     * - 200 OK + MeResponse (id, username, displayName, avatarUrl)
     * - 401 if access token is missing/invalid/expired (handled by security/filter).
     */
    @GetMapping("/me")
    public MeResponse me(@RequestAttribute("userId") Long userId) {
        User u = userRepository.findById(userId).orElseThrow();
        return new MeResponse(u.getId(), u.getUsername(), u.getDisplayName(), u.getAvatarUrl());
    }
}
