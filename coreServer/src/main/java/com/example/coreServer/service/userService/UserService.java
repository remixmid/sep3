package com.example.coreServer.service.userService;

import com.example.coreServer.dto.userDto.UserDto;
import com.example.coreServer.dto.userDto.UserRegistrationRequest;

public interface UserService {
    UserDto getUserById(Long id);

    UserDto getUserByUsername(String username);

    UserDto registerUser(UserRegistrationRequest request);
}
