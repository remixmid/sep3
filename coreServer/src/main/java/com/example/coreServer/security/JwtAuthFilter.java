package com.example.coreServer.security;

import com.example.coreServer.service.JWTService.JwtService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpHeaders;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.util.List;

@Slf4j
@Component
@RequiredArgsConstructor
public class JwtAuthFilter extends OncePerRequestFilter {

    private final JwtService jwtService;

    @Override
    protected boolean shouldNotFilter(HttpServletRequest request) {
        String path = request.getRequestURI();
        boolean skip = path.equals("/auth/login")
                || path.equals("/auth/register")
                || path.equals("/auth/refresh")
                || path.equals("/auth/logout");

        if (log.isDebugEnabled()) {
            log.debug("[JWT] shouldNotFilter={} method={} uri={}", skip, request.getMethod(), path);
        }
        return skip;
    }

    @Override
    protected void doFilterInternal(
            HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain
    ) throws ServletException, IOException {

        String method = request.getMethod();
        String uri = request.getRequestURI();
        String header = request.getHeader(HttpHeaders.AUTHORIZATION);

        if (log.isDebugEnabled()) {
            log.debug("[JWT] ENTER method={} uri={} authHeaderPresent={}",
                    method, uri, header != null && !header.isBlank());
        }

        if (header != null && header.startsWith("Bearer ")) {
            String token = header.substring(7);

            if (log.isDebugEnabled()) {
                log.debug("[JWT] Bearer token detected. tokenMasked={}", maskToken(token));
            }

            try {
                var payload = jwtService.parseAndValidate(token);

                if (log.isDebugEnabled()) {
                    log.debug("[JWT] Token valid. userId={} (will set SecurityContext)", payload.userId());
                }

                var auth = new UsernamePasswordAuthenticationToken(
                        payload.userId(),
                        null,
                        List.of(new SimpleGrantedAuthority("ROLE_USER"))
                );

                SecurityContextHolder.getContext().setAuthentication(auth);
                request.setAttribute("userId", payload.userId());

            } catch (Exception e) {
                log.warn("[JWT] Token invalid -> 401. method={} uri={} reason={} message={}",
                        method, uri, e.getClass().getSimpleName(), safeMsg(e));

                SecurityContextHolder.clearContext();
                response.sendError(HttpServletResponse.SC_UNAUTHORIZED);
                return;
            }
        } else {
            if (log.isDebugEnabled()) {
                log.debug("[JWT] No Bearer token. method={} uri={} headerValueMasked={}",
                        method, uri, header == null ? "<null>" : maskHeader(header));
            }
        }

        if (log.isDebugEnabled()) {
            var auth = SecurityContextHolder.getContext().getAuthentication();
            log.debug("[JWT] CONTINUE method={} uri={} authenticated={}",
                    method, uri, auth != null && auth.isAuthenticated());
        }

        filterChain.doFilter(request, response);
    }

    private static String maskToken(String token) {
        if (token == null || token.isBlank()) return "<empty>";
        if (token.length() <= 16) return token;
        return token.substring(0, 10) + "..." + token.substring(token.length() - 6);
    }

    private static String maskHeader(String header) {
        if (header == null || header.isBlank()) return "<empty>";
        String h = header.trim();
        if (h.startsWith("Bearer ")) {
            return "Bearer " + maskToken(h.substring(7));
        }
        if (h.length() <= 24) return h;
        return h.substring(0, 12) + "..." + h.substring(h.length() - 6);
    }

    private static String safeMsg(Exception e) {
        var m = e.getMessage();
        if (m == null) return "<null>";
        return m.length() > 200 ? m.substring(0, 200) + "..." : m;
    }
}
