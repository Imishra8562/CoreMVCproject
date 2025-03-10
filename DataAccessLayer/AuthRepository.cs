using CoreMVCproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

public class AuthRepository
{
    private readonly string _connectionString;

    public AuthRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Method to get all roles
    public List<Role> GetAllRoles()
    {
        var roles = new List<Role>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT Id, Name FROM Roles", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    roles.Add(new Role
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }

        return roles;
    }

    // Method to add a role
    public bool AddRole(Role role)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Check if the role already exists
            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Roles WHERE Name = @Name", connection);
            checkCommand.Parameters.AddWithValue("@Name", role.Name);
            int roleExists = (int)checkCommand.ExecuteScalar();

            if (roleExists == 0) // Role does not exist
            {
                // Insert the new role
                var insertCommand = new SqlCommand("INSERT INTO Roles (Name) VALUES (@Name)", connection);
                insertCommand.Parameters.AddWithValue("@Name", role.Name);
                insertCommand.ExecuteNonQuery();
                return true; // Role added successfully
            }
            else
            {
                // Role already exists
                return false; // Role not added
            }
        }
    }

    // Method to add a user
    public int AddUser(User user, out string errorMessage)
    {
        errorMessage = null;

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Check if the username already exists
            var checkUsernameCommand = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username",
                connection);

            checkUsernameCommand.Parameters.AddWithValue("@Username", user.Username);
            int usernameExists = (int)checkUsernameCommand.ExecuteScalar();

            if (usernameExists > 0)
            {
                errorMessage = "Username is already registered.";
                return -1; // Return a special value to indicate duplicate username
            }

            // Check if the email already exists
            var checkEmailCommand = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Email = @Email",
                connection);

            checkEmailCommand.Parameters.AddWithValue("@Email", user.Email);
            int emailExists = (int)checkEmailCommand.ExecuteScalar();

            if (emailExists > 0)
            {
                errorMessage = "Email is already registered.";
                return -2; // Return a special value to indicate duplicate email
            }

            // Insert the new user
            var insertCommand = new SqlCommand(
                "INSERT INTO Users (Username, PasswordHash, Email) VALUES (@Username, @PasswordHash, @Email); SELECT SCOPE_IDENTITY();",
                connection);

            insertCommand.Parameters.AddWithValue("@Username", user.Username);
            insertCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            insertCommand.Parameters.AddWithValue("@Email", user.Email);

            // Execute the query and return the newly created UserId
            return Convert.ToInt32(insertCommand.ExecuteScalar());
        }
    }
    // Method to assign a role to a user
    public bool AssignRoleToUser(int userId, int roleId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(
                "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            // Execute the query and return true if successful
            return command.ExecuteNonQuery() > 0;
        }
    }




    public bool ValidateUser(User user)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Query to check if the user exists with the provided username and password
                var checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash", connection);
                checkUser.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username });
                checkUser.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = user.PasswordHash });

                int userExists = (int)checkUser.ExecuteScalar();
                return userExists > 0; // Return true if user exists, otherwise false
            }
        }
        catch (Exception ex)
        {
            // Log the exception (e.g., using a logging framework)
            // For now, rethrow the exception
            throw new ApplicationException("An error occurred while validating the user.", ex);
        }
    }
}