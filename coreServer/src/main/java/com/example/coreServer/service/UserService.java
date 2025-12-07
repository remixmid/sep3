package com.example.coreServer.service;

import com.example.coreServer.dto.UserDto;
import com.example.coreServer.dto.UserRegistrationRequest;

public interface UserService {

    UserDto getUserById(Long id);

    UserDto getUserByUsername(String username);

    UserDto registerUser(UserRegistrationRequest request);
}
