using System;
using System.Runtime.Serialization;

namespace KafkaMessengerDemoApi.LearnerApi.Models
{
	[DataContract(Name = "LearnerSites")]
	public enum Site
	{
		MyED = 1,
		EpicRespondent = 2,
		EpicAdmin = 3,
		MLC = 4,
		CSC = 5,
		WebmasterSignIn = 6,
		Workplace3 = 7
	}

	public class LearnerAccount
	{
		public Site SiteID { get; set; }
		public int AccountID { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string AvatarFileName { get; set; } = "";
		public DateTime CreatedDT { get; set; }
		public DateTime? LastLoginDT { get; set; }
		public DateTime? StatusChangedDT { get; set; }
		public bool Searchable { get; set; }
		public string Organization { get; set; }
		public DateTime PasswordChangedDT { get; set; }
		public DateTime? DataConsentDT { get; set; }
		public DateTime? LastUpdateDT { get; set; }
		public string LastUpdateReason { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
		public bool? PublicProfileSearch { get; set; }
		public bool? CompanyProfileSearch { get; set; }
		public int? OrganizationID { get; set; }
		public int? LocationID { get; set; }
		public int? DepartmentID { get; set; }
		public int? CountryID { get; set; }
		public int? RegionID { get; set; }
		public bool HasCatalystData { get; set; } = false;
	}
}