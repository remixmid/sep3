package com.example.coreServer.controller;

import com.example.coreServer.dto.UserDto;
import com.example.coreServer.dto.UserRegistrationRequest;
import com.example.coreServer.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;


@RestController
@RequestMapping("/api/users")
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;

    /**
     * Get user by ID.
     * GET /api/users/123
     */
    @GetMapping("/{id}")
    public UserDto getUserById(@PathVariable("id") Long id) {
        return userService.getUserById(id);
    }

    /**
     * Get user by username.
     * GET /api/users/by-username?username=john
     */
    @GetMapping("/by-username")
    public UserDto getUserByUsername(@RequestParam("username") String username) {
        return userService.getUserByUsername(username);
    }

    /**
     * New user registration.
     * POST /api/users/register
     */
    @PostMapping("/register")
    public UserDto register(@RequestBody UserRegistrationRequest request) {
        return userService.registerUser(request);
    }
}
