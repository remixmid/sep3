package com.example.coreServer.web;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseCookie;
import org.springframework.stereotype.Component;

import java.time.Duration;

@Component
public class RefreshCookieFactory {

    @Value("${app.security.refresh.cookie-name}")
    private String cookieName;

    @Value("${app.security.refresh.cookie-samesite:Lax}")
    private String sameSite;

    @Value("${app.security.refresh.cookie-secure:false}")
    private boolean secure;

    @Value("${app.security.refresh.ttl-days}")
    private int ttlDays;

    public String name() { return cookieName; }

    public ResponseCookie create(String value) {
        return ResponseCookie.from(cookieName, value)
                .httpOnly(true)
                .secure(secure)
                .path("/auth")
                .maxAge(Duration.ofDays(ttlDays))
                .sameSite(sameSite)
                .build();
    }

    public ResponseCookie clear() {
        return ResponseCookie.from(cookieName, "")
                .httpOnly(true)
                .secure(secure)
                .path("/auth")
                .maxAge(Duration.ZERO)
                .sameSite(sameSite)
                .build();
    }
}
