using PetStoreProject.Models;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class AccountDetailViewModel
    {
        public int AccountId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public bool? Gender { get; set; }

        public string? Phone { get; set; }

        public DateOnly? DoB { get; set; }

        public string? Address { get; set; }

        public string Email { get; set; } = null!;

        public Role Role { get; set; }

        public bool? IsDelete { get; set; }

    }
}
