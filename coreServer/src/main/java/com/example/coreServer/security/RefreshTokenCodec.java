package com.example.coreServer.security;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.SecureRandom;

@Component
public class RefreshTokenCodec {

    private final SecureRandom rnd = new SecureRandom();
    private final String pepper;

    public RefreshTokenCodec(@Value("${app.security.refresh.pepper}") String pepper) {
        this.pepper = pepper;
    }

    public String generateRaw() {
        byte[] bytes = new byte[64];
        rnd.nextBytes(bytes);
        return bytesToHex(bytes);
    }

    public String hash(String raw) {
        try {
            MessageDigest md = MessageDigest.getInstance("SHA-256");
            byte[] dig = md.digest((raw + pepper).getBytes(StandardCharsets.UTF_8));
            return bytesToHex(dig);
        } catch (Exception e) {
            throw new IllegalStateException("SHA-256 not available", e);
        }
    }

    private static String bytesToHex(byte[] bytes) {
        StringBuilder sb = new StringBuilder(bytes.length * 2);
        for (byte b : bytes) sb.append(String.format("%02x", b));
        return sb.toString();
    }
}
