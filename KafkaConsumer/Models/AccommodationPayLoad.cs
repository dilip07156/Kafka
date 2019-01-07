using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumer.Models
{
    public class Lock
    {
        public bool enabled { get; set; }
        public string user { get; set; }
        public string workflowId { get; set; }
    }

    public class ProductStatus
    {
        public string status { get; set; }
        public string submitFor { get; set; }
        public string reason { get; set; }
        public string remark { get; set; }
        public DateFormat to { get; set; }
        public DateFormat from { get; set; }
        public List<Deactivated> deactivated { get; set; }
    }

    public class Minutes
    {
        public string unit { get; set; }
        public string value { get; set; }
    }

    public class Hours
    {
        public string unit { get; set; }
    }

    public class DurationFromProperty
    {
        public Minutes minutes { get; set; }
        public Hours hours { get; set; }
    }

    public class Miles
    {
        public string unit { get; set; }
        public string value { get; set; }
    }

    public class Km
    {
        public string unit { get; set; }
        public string value { get; set; }
    }

    public class DistanceFromProperty
    {
        public Miles miles { get; set; }
        public Km km { get; set; }
    }

    public class ApproxDuration
    {
        public string value { get; set; }
        public string unit { get; set; }
    }

    public class Direction
    {
        public string from { get; set; }
        public string nameOfPlace { get; set; }
        public string modeOfTransport { get; set; }
        public string transportType { get; set; }
        public DistanceFromProperty distanceFromProperty { get; set; }
        public ApproxDuration approxDuration { get; set; }
        public string description { get; set; }
        public string drivingDirection { get; set; }
        public string travelMode { get; set; }
        public string _id { get; set; }
        public DateFormat validityTo { get; set; }
        public DateFormat validityFrom { get; set; }
        public DurationFromProperty durationFromProperty { get; set; }
    }

    public class AgeForFromTo
    {
        public int to { get; set; }
        public int from { get; set; }
    }

    public class PassengerOccupancyType
    {
        public int maxPax { get; set; }
        public int maxCior { get; set; }
        public int maxCnb { get; set; }
        public int maxPaxWithExtraBeds { get; set; }
        public int maxAdults { get; set; }
        public int maxChild { get; set; }
        public string roomType { get; set; }
        public string roomCategory { get; set; }
        public string _id { get; set; }
        public AgeForFromTo ageForPaxExtraBed { get; set; }
        public AgeForFromTo ageBracketForCnb { get; set; }
        public AgeForFromTo ageForCior { get; set; }
    }

    public class PassengerOccupancy
    {
        public List<PassengerOccupancyType> type2 { get; set; }
        public List<PassengerOccupancyType> type1 { get; set; }
    }

    public class Facility
    {
        public bool isChargeable { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string _id { get; set; }
        public string operationalTimeFrom { get; set; }
        public string operationalTimeTo { get; set; }
        public string duration { get; set; }
    }

    public class Members
    {
        public string name { get; set; }
        public double age { get; set; }
        public string gender { get; set; }
        public string description { get; set; }
        public string languageSpoken { get; set; }
        public string interest { get; set; }
    }

    public class Childrens
    {
        public bool isPresent { get; set; }
        public int count { get; set; }
    }

    public class Pets
    {
        public bool isPresent { get; set; }
        public int count { get; set; }
        public string petTypes { get; set; }
    }

    public class HostFamilyDetails
    {
        public string description { get; set; }
        public int memberCount { get; set; }
        public Childrens childrens { get; set; }
        public Pets pets { get; set; }
        public bool isNonSmokinghouseHold { get; set; }
        public List<Members> members { get; set; }
    }

    public class LocationDetails
    {
        public string neighbourHoodDescription { get; set; }
        public string distanceFromCenter { get; set; }
        public string distanceFromPublicTransportation { get; set; }
    }

    public class CertificationDetails
    {
        public string certifiedHostId { get; set; }
        public string userCertificationDescription { get; set; }
        public string criminalRecordDescription { get; set; }
    }

    public class StaffContactInfo
    {
        public HostFamilyDetails hostFamilyDetails { get; set; }
        public LocationDetails locationDetails { get; set; }
        public CertificationDetails certificationDetails { get; set; }
    }

    public class Duration
    {
        public string description { get; set; }
        public string typeOfDesc { get; set; }
        public string _id { get; set; }
        public DateFormat to { get; set; }
        public DateFormat from { get; set; }
    }

    public class Overview
    {
        public bool isCompanyRecommended { get; set; }
        public string sellingTips { get; set; }
        public string highlights { get; set; }
        public string usp { get; set; }
        public List<string> hashTag { get; set; }
        public List<string> interest { get; set; }
        public List<Duration> duration { get; set; }
    }

    public class recommendedFor
    {
        public int RecommendedId { get; set; }
        public string RecommendedValue { get; set; }
    }

    public class Extra
    {
        public string description { get; set; }
        public string label { get; set; }
        public string _id { get; set; }
    }

    public class General
    {
        public string cfp { get; set; }
        public string yearBuilt { get; set; }
        public string yearRenovated { get; set; }
        public string awardsReceived { get; set; }
        public string internalRemarks { get; set; }
        public List<Extra> extras { get; set; }
        public string copiedFrom { get; set; }
    }

    public class Phone
    {
        public string number { get; set; }
        public string cityCode { get; set; }
        public string countryCode { get; set; }
    }

    public class Fax
    {
        public string number { get; set; }
        public string cityCode { get; set; }
        public string countryCode { get; set; }
    }

    public class ContactDetail
    {
        public string emailAddress { get; set; }
        public string website { get; set; }
        public string _id { get; set; }
        public Phone phone { get; set; }
        public Fax fax { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Address
    {
        public string houseNumber { get; set; }
        public string street { get; set; }
        public string street2 { get; set; }
        public string street3 { get; set; }
        public string street4 { get; set; }
        public string street5 { get; set; }
        public string zone { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string location { get; set; }
        public Geometry geometry { get; set; }
    }

    public class DateFormat
    {
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }

    public class AccomodationInfo
    {
        public string checkOutTime { get; set; }
        public string checkInTime { get; set; }
        public string displayName { get; set; }
        public bool isTwentyFourHourCheckout { get; set; }
        public int noOfRooms { get; set; }
        public int noOfFloors { get; set; }
        public string companyRating { get; set; }
        public string rating { get; set; }
        public string financeControlId { get; set; }
        public string name { get; set; }
        public bool isMysteryProduct { get; set; }
        public string productCatSubType { get; set; }
        public string companyId { get; set; }
        public string companyName { get; set; }
        public string companyProductId { get; set; }
        public string commonProductId { get; set; }
        public string chain { get; set; }
        public string brand { get; set; }
        public string affiliations { get; set; }
        public General general { get; set; }
        public List<ContactDetail> contactDetails { get; set; }
        public Address address { get; set; }
        public List<string> recommendedFor { get; set; }
        public DateFormat ratingDatedOn { get; set; }
        public FamilyDetails familyDetails { get; set; }
        public string resortType { get; set; }
    }

    public class FamilyDetails
    {
        public string familyName { get; set; }
        public string familyDescription { get; set; }
    }

    public class VersionHistory
    {
        public string versionRemark { get; set; }
        public string _id { get; set; }
        public string createdAt { get; set; }
        public int versionNumber { get; set; }
    }

    public class AccomodationData
    {
        public string _id { get; set; }
        public string createdBy { get; set; }
        public List<string> mysteryRoomsId { get; set; }
        public string Legacy_Htl_Id { get; set; }
        public int __v { get; set; }
        public List<VersionHistory> versionHistory { get; set; }
        public int versionNumber { get; set; }
        public Lock @lock { get; set; }
        public bool deleted { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime createdAt { get; set; }
        public string updatedBy { get; set; }
        public ProductStatus productStatus { get; set; }
        public List<Ancillary> ancillary { get; set; }
        public List<SafetyRegulations> safetyRegulations { get; set; }
        public List<Updates> updates { get; set; }
        public List<Media> media { get; set; }
        public List<Rule> rule { get; set; }
        public List<Landmark> landmark { get; set; }
        public List<InAndAround> inAndAround { get; set; }
        public List<Direction> directions { get; set; }
        public PassengerOccupancy passengerOccupancy { get; set; }
        public List<Facility> facility { get; set; }
        public StaffContactInfo staffContactInfo { get; set; }
        public Overview overview { get; set; }
        public AccomodationInfo accomodationInfo { get; set; }
        public List<string> roomIds { get; set; }
    }

    public class Ancillary
    {
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public DateFormat from { get; set; }
        public DateFormat to { get; set; }
    }

    public class SafetyRegulations
    {
        public string category { get; set; }
        public string name { get; set; }
        public string remarks { get; set; }
        public bool value { get; set; }
        public DateFormat lastUpdated { get; set; }
    }

    public class Updates
    {
        public string updateCategory { get; set; }
        public string description { get; set; }
        public string source { get; set; }
        public bool sendUpdates { get; set; }
        public bool displayUpdatesOnVoucher { get; set; }
        public DescriptionType descriptionType { get; set; }
        public Display display { get; set; }
        public ModeOfCommunication modeOfCommunication { get; set; }
    }

    public class DescriptionType
    {
        public bool @internal { get; set; }
        public bool external { get; set; }
    }

    public class ModeOfCommunication
    {
        public bool email { get; set; }
        public bool sms { get; set; }
    }

    public class Display
    {
        public DateFormat from { get; set; }
        public DateFormat to { get; set; }
    }

    public class Rule
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Media
    {
        public string mediaId { get; set; }
        public string fileCategory { get; set; }
        public string fileType { get; set; }
        public string fileName { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string mediaPosition { get; set; }
        public DateFormat from { get; set; }
        public DateFormat to { get; set; }
    }

    public class Landmark
    {
        public string placeCategory { get; set; }
        public string description { get; set; }
        public string placeName { get; set; }
        public string travelMode { get; set; }
        public string _id { get; set; }
        public DistanceFromProperty distanceFromProperty { get; set; }
    }

    public class InAndAround
    {
        public string placeCategory { get; set; }
        public string description { get; set; }
        public string placeName { get; set; }
        public string travelMode { get; set; }
        public string locationHighlights { get; set; }
        public string _id { get; set; }
        public DurationFromProperty durationFromProperty { get; set; }
        public DistanceFromProperty distanceFromProperty { get; set; }
    }

    public class Amenity
    {
        public string name { get; set; }
        public string type { get; set; }
        public string desc { get; set; }
        public string _id { get; set; }
        public bool isChargeable { get; set; }
    }

    public class Deactivated
    {
        public string reason { get; set; }
        public string marketName { get; set; }
        public string marketId { get; set; }
        public string _id { get; set; }
        public DateFormat to { get; set; }
        public DateFormat from { get; set; }
    }

    public class AccomodationRoomData
    {
        public string _id { get; set; }
        public string accomodationId { get; set; }
        public string companyRoomCategory { get; set; }
        public int __v { get; set; }
        public List<VersionHistory> versionHistory { get; set; }
        //public string LEGACY_HTL_ID { get; set; }
        public string roomId { get; set; }
        public bool isMysteryRoom { get; set; }
        public string roomDescription { get; set; }
        public string roomUnit { get; set; }
        public int roomSize { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string view { get; set; }
        public int noOfRooms { get; set; }
        public int noOfInterconnectingRooms { get; set; }
        public string numberAndNameOfUnit { get; set; }
        public string floorsNameAndNameOfUnit { get; set; }
        public string numberAndNameOfRoom { get; set; }
        public double roomSizeInSquareFeet { get; set; }
        public double roomSizeInSquareMeter { get; set; }
        public string bathroomType { get; set; }
        public string bedTypeByUnit { get; set; }
        public string roomDecor { get; set; }
        public string roomSubType { get; set; }
        public string floorName { get; set; }
        public int floorNo { get; set; }
        public string bedType { get; set; }
        public int versionNumber { get; set; }
        public bool deleted { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime createdAt { get; set; }
        public List<Amenity> amenities { get; set; }
    }

    public class AccommodationPayload
    {
        public AccomodationData accomodationData { get; set; }
        public List<AccomodationRoomData> accomodationRoomData { get; set; }
    }
}
