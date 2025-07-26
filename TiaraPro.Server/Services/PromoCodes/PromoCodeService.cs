using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.DTOs;
using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer;
using System.Collections.Generic;
using System.Linq;

namespace TiaraPro.Server.Services.PromoCodes
{
    public interface IPromoCodeService
    {
        Task<ValidatePromoCodeResponse> ValidatePromoCodeAsync(ValidatePromoCodeRequest request);
        Task<PromoCodeResponse> CreatePromoCodeAsync(PromoCode promoCode);
        Task<PromoCodeResponse> GetPromoCodeAsync(string code);
        Task<bool> CreateUserPromoCodeAsync(UserPromoCodeUsage userPromoCode);
        Task<IEnumerable<PromoCodeResponse>> GetAllPromoCodesAsync();
        Task<PromoCode> GetPromoCodeById(int id);
    }

    public class PromoCodeService : IPromoCodeService
    {
        private readonly TiaraDbContext _context;

        public PromoCodeService(TiaraDbContext context)
        {
            _context = context;
        }

        public async Task<PromoCode> GetPromoCodeById(int id)
        {
            try
            {
                var promoCode = await _context.PromoCodes
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (promoCode == null)
                {
                    Console.WriteLine($"Promo code with ID {id} not found.");
                    return new PromoCode();
                }
                return promoCode;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching promo code by ID: {ex.Message}");
                return new PromoCode();
            }

        }
        public async Task<ValidatePromoCodeResponse> ValidatePromoCodeAsync(ValidatePromoCodeRequest request)
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code == request.Code);

            if (promoCode == null)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = "Invalid promo code"
                };
            }

            // Check if user has already used this promo code
            var userUsedPromo = await _context.UserPromoCodeUsages
                .AnyAsync(upc => upc.UserId == request.UserId && upc.PromoCodeId == promoCode.Id);
            if (userUsedPromo)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = "You have already used this promo code."
                };
            }

            if (!promoCode.IsActive)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = "Promo code is no longer active"
                };
            }

            if (DateTime.UtcNow < promoCode.StartDate || DateTime.UtcNow > promoCode.EndDate)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = "Promo code has expired"
                };
            }

            if (promoCode.MaxUses.HasValue && promoCode.CurrentUses >= promoCode.MaxUses)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = "Promo code has reached its maximum usage limit"
                };
            }

            if (promoCode.MinimumOrderAmount.HasValue && request.OrderAmount < promoCode.MinimumOrderAmount.Value)
            {
                return new ValidatePromoCodeResponse
                {
                    IsValid = false,
                    Message = $"Minimum order amount of {promoCode.MinimumOrderAmount.Value} EGP required"
                };
            }

            var finalAmount = request.OrderAmount - promoCode.DiscountAmount;
            if (finalAmount < 0) finalAmount = 0;

            return new ValidatePromoCodeResponse
            {
                IsValid = true,
                Message = "Promo code applied successfully",
                DiscountAmount = (decimal)((double)request.OrderAmount * ((double)promoCode.DiscountAmount / 100.0)),
                FinalAmount = finalAmount,
                PromoCodeId = promoCode.Id
            };
        }

        public async Task<PromoCodeResponse> CreatePromoCodeAsync(PromoCode promoCode)
        {
            promoCode.StartDate = DateTime.SpecifyKind(promoCode.StartDate, DateTimeKind.Utc);
            promoCode.EndDate = DateTime.SpecifyKind(promoCode.EndDate, DateTimeKind.Utc);
            promoCode.CreatedAt = DateTime.UtcNow;

            await _context.PromoCodes.AddAsync(promoCode);
            await _context.SaveChangesAsync();

            return MapToResponse(promoCode);
        }

        public async Task<PromoCodeResponse> GetPromoCodeAsync(string code)
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code == code);

            return promoCode != null ? MapToResponse(promoCode) : null;
        }

        public async Task<bool> CreateUserPromoCodeAsync(UserPromoCodeUsage userPromoCode)
        {
            try
            {
                if (userPromoCode == null)
                {
                    Console.WriteLine("UserPromoCode is null");
                    return false;
                }

                // Get the promo code to update its usage count
                var promoCode = await _context.PromoCodes.FindAsync(userPromoCode.PromoCodeId);
                if (promoCode == null)
                {
                    Console.WriteLine($"Promo code with ID {userPromoCode.PromoCodeId} not found");
                    return false;
                }

                // Check if user has already used this promo code
                var existingUsage = await _context.UserPromoCodeUsages
                    .AnyAsync(upc => upc.UserId == userPromoCode.UserId && upc.PromoCodeId == userPromoCode.PromoCodeId);

                if (existingUsage)
                {
                    Console.WriteLine($"User {userPromoCode.UserId} has already used promo code {userPromoCode.PromoCodeId}");
                    return false;
                }

                promoCode.CurrentUses++;
                await _context.UserPromoCodeUsages.AddAsync(userPromoCode);

                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully created UserPromoCode for user {userPromoCode.UserId} and promo code {userPromoCode.PromoCodeId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating UserPromoCode: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<IEnumerable<PromoCodeResponse>> GetAllPromoCodesAsync()
        {
            var promoCodes = await _context.PromoCodes.ToListAsync();
            return promoCodes.Select(p => new PromoCodeResponse
            {
                Id = p.Id,
                Code = p.Code,
                DiscountAmount = p.DiscountAmount,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                MaxUses = p.MaxUses,
                CurrentUses = p.CurrentUses,
                MinimumOrderAmount = p.MinimumOrderAmount
            });
        }

        private static PromoCodeResponse MapToResponse(PromoCode promoCode)
        {
            return new PromoCodeResponse
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                DiscountAmount = promoCode.DiscountAmount,
                StartDate = promoCode.StartDate,
                EndDate = promoCode.EndDate,
                IsActive = promoCode.IsActive,
                MaxUses = promoCode.MaxUses,
                CurrentUses = promoCode.CurrentUses,
                MinimumOrderAmount = promoCode.MinimumOrderAmount
            };
        }
    }
}