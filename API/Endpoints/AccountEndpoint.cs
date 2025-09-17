using System;
using System.Dynamic;
using API.Common;
using API.DTOs;
using API.Extentions;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class AccountEndpoint
{
    public static RouteGroupBuilder MapAccountEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/api/account").WithTags("account");
        group.MapPost("/register", async(HttpContext context, UserManager < AppUser > userManager,
         [FromForm] string fullName, [FromForm] string email, [FromForm] string password, [FromForm] string userName, [FromForm] IFormFile? profileImage) =>
        {
            var userFromDB = await userManager.FindByEmailAsync(email);

            if (userFromDB != null)
            {
                return Results.BadRequest(Response<string>.Failure("Usuário já existe"));
            }

            if (profileImage == null)
            {
                return Results.BadRequest(Response<string>.Failure("Imagem de perfil é obrigatória"));
            }

            var picture = await FileUpload.Upload(profileImage);

            picture = $"{context.Request.Scheme}://{context.Request.Host}/uploads/{picture}";

            var user = new AppUser
            {
                FullName = fullName,
                Email = email,
                UserName = userName,
                ProfileImage = picture
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest(Response<string>.Failure(errors));
            }

            return Results.Ok(Response<string>.Success("", "Usuario cadastrado com sucesso"));
        }).DisableAntiforgery();
        
        group.MapPost("/login", async(UserManager <AppUser> userManager, TokenService tokenService, LoginDTO dto) =>
        {
            if (dto == null)
            {
                return Results.BadRequest(Response<string>.Failure("Detalhes de login inválidos"));
            }

            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Results.BadRequest(Response<string>.Failure("Usuário não encontrado"));
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
            {
                return Results.BadRequest(Response<string>.Failure("Senha incorreta"));
            }

            var token = tokenService.GenerateToken(user.Id, user.UserName!);

            return Results.Ok(Response<string>.Success(token, "Login realizado com sucesso"));
        }).DisableAntiforgery();

        group.MapGet("/user", async (HttpContext context, UserManager<AppUser> userManager) =>
        {
            var currentLoggedUserId = context.User.GetUserId()!;

            var currentLoggedInUser = await userManager.Users.SingleOrDefaultAsync(x => x.Id == currentLoggedUserId.ToString());

            return Results.Ok(Response<AppUser>.Success(currentLoggedInUser!, "Usuário logado"));
        }).RequireAuthorization();

        return group;
    }
}
