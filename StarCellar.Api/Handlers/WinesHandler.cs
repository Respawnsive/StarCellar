using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using StarCellar.Api.Data;
using StarCellar.Api.Utils;

namespace StarCellar.Api.Handlers
{
    internal static class WinesHandler
    {
        internal static async Task<IResult> GetAllWines(AppDbContext appContext,
            IHttpContextAccessor httpContextAccessor)
        {
            //if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
            //    return TypedResults.BadRequest(errMsg);

            return TypedResults.Ok(await appContext
                .Wines
                //.Where(wine => wine.OwnerId == user.Id)
                .Select(wine => new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                    wine.Score, wine.OwnerId)).ToArrayAsync());
        }

        internal static async Task<IResult> GetWine(Guid id, AppDbContext appContext,
            IHttpContextAccessor httpContextAccessor)
        {
            //if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
            //    return TypedResults.BadRequest(errMsg);

            return await appContext.Wines.FindAsync(id)
                is { } wine 
                   //&& wine.OwnerId == user.Id
                ? TypedResults.Ok(new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                    wine.Score, wine.OwnerId))
                : TypedResults.NotFound();
        }

        internal static async Task<IResult> CreateWine(WineDTO wineDto, 
            AppDbContext appContext,
            IHttpContextAccessor httpContextAccessor)
        {
            if (wineDto is null) 
                return TypedResults.BadRequest("Must include a Wine.");

            if (!MiniValidator.TryValidate(wineDto, out var errors)) 
                return TypedResults.BadRequest(errors);

            //if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
            //    return TypedResults.BadRequest(errMsg);

            var owner = await appContext.Users.FirstAsync(); // To comment while authenticated

            var (id, name, description, imageUrl, stock, score, _) = wineDto;

            var ownerId = owner.Id;

            if (id == Guid.Empty) 
                id = Guid.NewGuid();

            //var owner = await appContext.Users.FindAsync(ownerId);
            //if (owner is null) 
            //    return TypedResults.NotFound($"User with Id {ownerId} not found.");

            var wineFromDb = await appContext.Wines.FindAsync(id);
            if (wineFromDb is not null) 
                return TypedResults.Conflict("A wine with this ID already exists.");

            var wine = new Wine(id, name, description, imageUrl, stock, score, ownerId, owner);

            appContext.Wines.Add(wine);
            await appContext.SaveChangesAsync();

            wineDto = new WineDTO(wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock,
                wine.Score, wine.OwnerId);

            return TypedResults.Created($"/wines/{wineDto.Id}", wineDto);
        }

        internal static async Task<IResult> UpdateWine(Guid id, WineDTO wineDto, AppDbContext appContext,
            IHttpContextAccessor httpContextAccessor)
        {
            if (wineDto is null)
                return TypedResults.BadRequest("Must include a Todo.");

            if (!MiniValidator.TryValidate(wineDto, out var errors))
                return TypedResults.BadRequest(errors);

            //if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
            //    return TypedResults.BadRequest(errMsg);

            var wine = await appContext.Wines.FindAsync(id);
            if (wine is null 
                //|| wine.OwnerId != user.Id
                ) 
                return TypedResults.NotFound();

            wine.Name = wineDto.Name;
            wine.Description = wineDto.Description;
            wine.ImageUrl = wineDto.ImageUrl;
            wine.Stock = wineDto.Stock;
            wine.Score = wineDto.Score;

            await appContext.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        internal static async Task<IResult> DeleteWine(Guid id, AppDbContext appContext,
            IHttpContextAccessor httpContextAccessor)
        {
            //if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
            //    return TypedResults.BadRequest(errMsg);

            if (await appContext.Wines.FindAsync(id) is not { } wine 
                //|| wine.OwnerId != user.Id
                ) 
                return TypedResults.NotFound();

            appContext.Wines.Remove(wine);
            await appContext.SaveChangesAsync();

            return TypedResults.NoContent();

        }
    }
}
