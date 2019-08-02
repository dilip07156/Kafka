using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumer.Models
{

    public class ActivityDataPayLoad
    {
        public ActivityMainDef activityMainData { get;set;}

        public List<ActivitysubDef> activitySubData { get; set; }
    }

    public class ActivitysubDef
    {

    }

    public class ActivityMainDef
    {
        public string _id { get; set; }

        public string companyId { get; set; }

        public string companyName { get; set; }

        public ProductInfo productInfo { get; set; }

        public ActOverview overview { get; set; }

        public List<GeneralInfo> generalInfo { get; set; }

        public MiscellaneousInfo miscellaneousInfo { get; set; }

        public List<Facilities> facilities { get; set; }

        public List<Shopping> shopping { get; set; }

        public List<ParkMap> parkMap { get; set; }

        public List<EventsAndTourInfo> eventsAndTourInfo { get; set; }

        public List<DiningInfo> diningInfo { get; set; }

        public MainOtherInfo otherInfo { get; set; }

        public PreArrivaltips preArrivaltips { get; set; }

        public List<SuggestedAccommodation> suggestedAccommodation { get; set; }

        public List<ActMedia> media { get; set; }

        public ProductPolicy productPolicy { get; set; }

        public CustomDefineFields customDefineFields { get; set; }

        public ActProductStatus productStatus { get; set; }

        public ProductAssociation productAssociation { get; set; }

        public DateTime createdAt { get; set; }

        public string createdBy { get; set; }

        public DateTime lastUpdated { get; set; }

        public string updatedBy { get; set; }

        public bool deleted { get; set; }

        public Lock @lock { get;set;}

        public string recordCompanyId { get; set; }
    }

    public class ProductInfo
    {
        public string productCategory { get; set; }

        public string productCategorySubtype { get; set; }

        public string productName { get; set; }

        public string productType { get; set; }

        public string companyProductId { get; set; }

        public string financeControlId { get; set; }

        public string commonProductId { get; set; }

        public string displayName { get; set; }

        public string affiliation { get; set; }

        public string modeOfTransport { get; set; }

        public string country { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string brand { get; set; }

        public string chain { get; set; }

        public string copiedFrom { get; set; }

        public List<ActMedia> media { get; set; }

    }


    public class ActMedia
    {
        public string mediaId { get; set; }

        public string fileCategory { get; set; }

        public string fileName { get; set; }

        public string fileDescription { get; set; }

        public DatetimeFormat from { get; set; }

        public DatetimeFormat to { get; set; }
    }

    public class ActOverview
    {
        public List<OverViewDetails> overViewDetails { get; set; }

        public string interest { get; set; }

        public string highlights { get; set; }
    }

    public class OverViewDetails
    {
        public DatetimeFormat from { get; set; }

        public DatetimeFormat to { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }
    }

    public class GeneralInfo
    {
        public string name { get; set; }

        public string description { get; set; }

        public AdditionalInfo additionalInfo { get; set; }

    }

    public class AdditionalInfo
    {
        public string history { get; set; }

        public string yearBuilt { get; set; }

        public string yearRenovated { get; set; }

        public string stories { get; set; }

        public string aboutUs { get; set; }

        public string celebrityComments { get; set; }

        public string awardsReceived { get; set; }
    }

    public class MiscellaneousInfo
    {
        public List<AddDescInfo> addDescInfo { get; set; }

        public List<AddNameDesc> addNameDesc { get; set; }
    }

    public class AddDescInfo
    {
        public string miscRestrictionsDesc { get; set; }

        public string isAllowed { get; set; }

        public string miscNote { get; set; }
    }

    public class AddNameDesc
    {
        public string miscName { get; set; }

        public string miscDesc { get; set; }
    }

    public class Facilities
    {
        public string facilityCategory { get; set; }

        public string facilityType { get; set; }

        public string desc { get; set; }

        public DatetimeFormat from { get; set; }

        public DatetimeFormat to { get; set; }
    }

    public class Shopping
    {
        public string shopName { get; set; }

        public string typeOfMerchandise { get; set; }

        public string desc { get; set; }

        public ActMedia media { get; set; }

        public ShoppingAdditionalInfo additionalInfo { get; set; }

        public string mapId { get; set; }

    }

    public class ShoppingAdditionalInfo
    {
        public string nameOfItem { get; set; }

        public string location { get; set; }

        public string whereAbouts { get; set; }

        public string paymentDetails { get; set; }

        public string contact { get; set; }

        public List<string> daysOfWeek { get; set; }

        public string reservation { get; set; }

        public string reservationInfo { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public string toDate { get; set; }

        public OpeningDetails openingDetails { get; set; }

        public ClosingDetails closingDetails { get; set; }
    }

    public class OpeningDetails
    {
        public string openingTime { get; set; }

        public string notes { get; set; }
    }

    public class ClosingDetails
    {
        public string closingTime { get; set; }

        public string notes { get; set; }
    }

    public class ParkMap
    {
        public string nameOfMap { get; set; }

        public ActMedia media { get; set; }

        public MapLocation mapLocation { get; set; }
    }

    public class MapLocation
    {
        public string latitude { get; set; }

        public string longitude { get; set; }
    }

    public class EventsAndTourInfo
    {
        public string type { get; set; }

        public string name { get; set; }

        public string duration { get; set; }

        public string desc { get; set; }

        public string whereAbouts { get; set; }

        public string location { get; set; }

        public bool reservations { get; set; }

        public ActMedia media { get; set; }

        public ContactDetails contactDetails { get; set; }

        public string interest { get; set; }

        public string guestPolicy { get; set; }

        public string unit { get; set; }

        public string guestHeight { get; set; }

        public AgeRange ageRange { get; set; }

        public string thrillLevels { get; set; }

        public string accessibility { get; set; }

        public string knowBeforeYouGo { get; set; }

        public string remarks { get; set; }

        public DaysOfOperations daysOfOperations { get; set; }

        public MapLocation mapLocation { get; set; }

        public string mediaId { get; set; }
    }

    public class DaysOfOperations
    {
        public List<string> daysOfWeek { get; set; }

        public DateTime fromDate { get; set; }

        public DateTime todate { get; set; }

        public string fromTime { get; set; }

        public string toTime { get; set; }


    }

    public class AgeRange
    {
        public string fromYear { get; set; }

        public string fromMonth { get; set; }

        public string toMonth { get; set; }

        public string toYear { get; set; }

    }

    public class ContactDetails
    {
        public TelephoneNumber telephoneNumber { get; set; }
    }

    public class TelephoneNumber
    {
        public string countryCode { get; set; }

        public string cityCode { get; set; }

        public string teleNumber { get; set; }
    }

    public class DiningInfo
    {
        public string restaurantName { get; set; }

        public string desc { get; set; }

        public string whereAbouts { get; set; }

        public string locations { get; set; }

        public OtherInfo otherInfo { get; set; }

        public DiningInfoContactDetails contactDetails { get; set; }

        public ReservationContactDetails reservationContactDetails { get; set; }

        public MenuInfo menuInfo { get; set; }

        public SeatingInfo seatingInfo { get; set; }

        public RestaurantTiminigs restaurantTiminigs { get; set; }
    }

    public class RestaurantTiminigs
    {
        public string eventType { get; set; }

        public string mealType { get; set; }

        public string dateRange { get; set; }//dilip

        public DateTime fromDate { get; set; }

        public DateTime toDate { get; set; }

        public List<string> daysOfWeek { get; set; }

        public string openingTime { get; set; }

        public string openingNotes { get; set; }

        public string closeTime { get; set; }

        public string closeNotes { get; set; }

        public string lastOrderTime { get; set; }

        public string remarks { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public MapLocation mapLocation { get; set; }
    }

    public class SeatingInfo
    {
        public string seatType { get; set; }

        public string seatingArrangements { get; set; }

        public string purpose { get; set; }

        public string RoomName { get; set; }

        public string seatingCapacity { get; set; }

        public ActMedia media { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public string mediaId { get; set; }
    }

    public class MenuInfo
    {
        public string menuName { get; set; }

        public string menuItemName { get; set; }

        public string menuItemDesc { get; set; }

        public string spiceLeveL { get; set; }

        public string unit { get; set; }

        public ActMedia media { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }

        public ValidityInfo validityInfo { get; set; }

        public string mediaId { get; set; }

        public MapLocation mapLocation { get; set; }
    }

    public class ValidityInfo
    {
        public List<string> daysOfWeek { get; set; }

        public string fromTime { get; set; }

        public string toTime { get; set; }
    }

    public class ReservationContactDetails
    {
        public bool AcceptWalking { get; set; }

        public bool reservationRequired { get; set; }

        public string reservationFor { get; set; }

        public ReservationContactDetails_ContactDetails contactDetails { get; set; }
    }

    public class ReservationContactDetails_ContactDetails
    {
        public TelephoneNumber telephoneNumber { get; set; }
        public FaxNumber faxNumber { get; set; }

        public string websiteURL { get; set; }

        public string emailAddress { get; set; }

        public string notesOnReservation { get; set; }
    }

    public class DiningInfoContactDetails
    {
        public TelephoneNumber telephoneNumber { get; set; }
        public FaxNumber faxNumber { get; set; }

        public string websiteURL { get; set; }

        public string emailAddress { get; set; }

        public string notes { get; set; }
    }

    public class FaxNumber
    {
        public string countryCode { get; set; }

        public string cityCode { get; set; }

        public string faxNumber { get; set; }
    }

    public class OtherInfo
    {
        public bool airCondition { get; set; }

        public bool smokingAllowed { get; set; }

        public string smokingNote { get; set; }

        public bool alcoholServed { get; set; }

        public bool petsAllowed { get; set; }

        public string petNote { get; set; }

        public bool childrenAllowed { get; set; }

        public string childrenNote { get; set; }

        public string seatingCapacity { get; set; }

        public string ambience { get; set; }

        public bool outsideBeverageOaArlcoholAllowed { get; set; }

        public bool corkageApplicable { get; set; }

        public string theme { get; set; }

        public string cusineSpecialityContinental { get; set; }

        public bool barsAndLoungesAvailable { get; set; }

        public string noteOfTip { get; set; }

        public bool validAdmisionRequired { get; set; }

        public bool isDiscountAnnualPassHolder { get; set; }

        public string typeOfDining { get; set; }

        public string style { get; set; }

        public string typeOfService { get; set; }

        public string mealType { get; set; }

        public string cusineType { get; set; }

        public string menuType { get; set; }

        public string foodType { get; set; }

    }

    public class MainOtherInfo
    {
        public List<Detail> detail { get; set; }
    }

    public class Detail
    {
        public string typeOfInformation { get; set; }

        public string nameOfInformation { get; set; }

        public string description { get; set; }
    }

    public class PreArrivaltips
    {
        public List<PreArrivaltipsOverview> overview { get; set; }

        public Dining dining { get; set; }
    }

    public class Dining
    {
        public string reservations { get; set; }

        public string characterDining { get; set; }

        public string casualDining { get; set; }

        public string signatureDining { get; set; }

        public string splAndUniqueDining { get; set; }

        public string quickServiceDining { get; set; }

        public string barsAndLounges { get; set; }

        public string disneyDining { get; set; }
    }

    public class PreArrivaltipsOverview
    {
        public string name { get; set; }

        public string desc { get; set; }

        public string visitMonth { get; set; }

        public string visitMonthDesc { get; set; }
    }

    public class SuggestedAccommodation
    {
        public string productCatSubType { get; set; }

        public string productId { get; set; }

        public string productName { get; set; }

        public string country { get; set; }

        public string city { get; set; }

        public string desc { get; set; }
    }

    public class ProductPolicy
    {
        public List<string> policyId { get; set; }
    }

    public class CustomDefineFields
    {
        public string customAttributeID { get; set; }

        public List<Columns> columns { get; set; }
    }

    public class Columns
    {
        public string label { get; set; }

        public string value { get; set; }
    }

    public class ActProductStatus
    {
        public List<Products> products { get; set; }

        public string status { get; set; }
    }

    public class Products
    {
        public string companyMarket { get; set; }

        public DatetimeFormat fromDate { get; set; }

        public string reason { get; set; }

        public string status { get; set; }
    }   

    public class ProductAssociation
    {
        public bool isAssociationWithProduct { get; set; }

        public string productCategory { get; set; }

        public string productCategorySubType { get; set; }

        public string country { get; set; }

        public string city { get; set; }

        public string productId { get; set; }

        public PropertyAddress propertyAddress { get; set; }

        public List<ProdAssociationContactDetails> contactDetails { get; set; }

        public GeoLocation geoLocation { get; set; }

        public List<HowToReach> howToReach { get; set; }

    }

    public class HowToReach
    {
        public string from { get; set; }

        public string nameOfPlace { get; set; }

        public string travelMode { get; set; }

        public DistanceFromProp distanceFromProperty { get; set; }

        public DurationFromProperty durationFromProperty { get; set; }

        public string description { get; set; }

        public string drivingDirection { get; set; }

        public DatetimeFormat validityFrom { get; set; }

        public DatetimeFormat validityTo { get; set; }
    }

    public class DatetimeFormat
    {
        public double day { get; set; }

        public double month { get; set; }

        public double year { get; set; }
    }

    public class DistanceFromProp
    {
        public KeyValue km { get;set;}

        public KeyValue miles { get; set; }
    }

    public class KeyValue
    {
        public string value { get; set; }

        public string unit { get; set; }
    }
    public class GeoLocation
    {
        public double latitude { get; set; }

        public double longitude { get; set; }
    }

    public class ProdAssociationContactDetails
    {
        public Phone phone { get; set; }

        public Fax fax { get; set; }

        public string emailAddress { get; set; }

        public string websiteUrl { get; set; }
    }

    public class PropertyAddress
    {
        public PropAdd address { get; set; }
    }

    public class PropAdd
    {
        public string houseNumber { get; set; }
        public string street { get; set; }
        public string street2 { get; set; }
        public string street3 { get; set; }
        public string street4 { get; set; }
        public string street5 { get; set; }        
        public string postalCode { get; set; }
        public string suburbsOrDownTown { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string location { get; set; }       
    }
}
