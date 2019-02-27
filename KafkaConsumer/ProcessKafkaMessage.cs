using KafkaConsumer.MDMSVC;
using KafkaConsumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    public static class ProcessKafkaMessage
    {
        public static async void InsertInto_StgKafka(Confluent.Kafka.Message<Confluent.Kafka.Null, string> msg)
        {
            StartProcess sp = new StartProcess();
            try
            {
                await Proxy.Post<DC_Message, DC_Stg_Kafka>(System.Configuration.ConfigurationManager.AppSettings["Kafka_Insert"], new DC_Stg_Kafka()
                {
                    Error = msg.Error.Reason,
                    TopicPartion = msg.TopicPartition.Partition.ToString(),
                    Key = Convert.ToString(msg.Key),
                    //TimeStamp = msg.Timestamp.UtcDateTime,
                    PayLoad = msg.Value,
                    Offset = msg.Offset.Value.ToString(),
                    Partion = msg.Partition.ToString(),
                    Create_User = "KafkaConsumer",
                    Create_Date = DateTime.Now,
                    Row_Id = Guid.NewGuid(),
                    Topic = msg.Topic,
                    TopicPartionOffset = msg.TopicPartition.Partition.ToString()
                });
            }
            catch (Exception Ex)
            {
                sp.Log("Execption occurs InsertInto_StgKafka Method");
                sp.Log(Ex.ToString());
                //throw;
            }
        }


        public static void InsertInto_StgKafkaV2(Confluent.Kafka.Message<Confluent.Kafka.Null, string> msg)
        {
            StartProcess sp = new StartProcess();
            try
            {
                //await Proxy.Post<DC_Message, DC_Stg_Kafka>(System.Configuration.ConfigurationManager.AppSettings["Kafka_Insert"], new DC_Stg_Kafka()
                //{
                //    Error = msg.Error.Reason,
                //    TopicPartion = msg.TopicPartition.Partition.ToString(),
                //    Key = Convert.ToString(msg.Key),
                //    //TimeStamp = msg.Timestamp.UtcDateTime,
                //    PayLoad = msg.Value,
                //    Offset = msg.Offset.Value.ToString(),
                //    Partion = msg.Partition.ToString(),
                //    Create_User = "KafkaConsumer",
                //    Create_Date = DateTime.Now,
                //    Row_Id = Guid.NewGuid(),
                //    Topic = msg.Topic,
                //    TopicPartionOffset = msg.TopicPartition.Partition.ToString()
                //});

                using (ConsumerEntities context = new ConsumerEntities())
                {

                    Stg_Kafka sk = new Stg_Kafka()
                    {
                        Row_Id = Guid.NewGuid(),
                        Topic = msg.Topic,
                        PayLoad = msg.Value,
                        Error = msg.Error.Reason,
                        Key = Convert.ToString(msg.Key),
                        Offset = msg.Offset.Value.ToString(),
                        Partion = msg.Partition.ToString(),
                        //TimeStamp = KafkaInfo.TimeStamp.get,
                        TopicPartion = msg.TopicPartition.Partition.ToString(),
                        TopicPartionOffset = msg.TopicPartition.Partition.ToString(),
                        Create_User = "KafkaConsumer",
                        Create_Date = DateTime.Now,
                        Process_User = null,
                        Process_Date = null,

                    };
                    context.Stg_Kafka.Add(sk);

                    context.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                sp.Log("Execption occurs InsertInto_StgKafkaV2 Method");
                sp.Log(Ex.ToString());
                //throw;
            }

        }

        public static void InsertInto_StgKafkaTestV2()
        {
            StartProcess sp = new StartProcess();
            try
            {
               

                using (ConsumerEntities context = new ConsumerEntities())
                {

                    Stg_Kafka sk = new Stg_Kafka()
                    {
                        Row_Id = Guid.NewGuid(),
                        Topic = "TestGS",
                        PayLoad = "TestGS",
                        Error = "",
                        Key = Convert.ToString("123"),
                        Offset = "0",
                        Partion = "0",
                        //TimeStamp = KafkaInfo.TimeStamp.get,
                        TopicPartion = "Test",
                        TopicPartionOffset = "Test",
                        Create_User = "KafkaConsumer",
                        Create_Date = DateTime.Now,
                        Process_User = null,
                        Process_Date = null,

                    };
                    context.Stg_Kafka.Add(sk);

                    context.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                sp.Log("Execption occurs InsertInto_StgKafkaV2 Method");
                sp.Log(Ex.ToString());
                //throw;
            }

        }

        public static bool Process_StgKafkaData()
        {
            bool Result = false;
            List<DC_Stg_Kafka> objUnprocessedData = new List<DC_Stg_Kafka>();
            objUnprocessedData = GetAllUnprocessedData();

            if (objUnprocessedData.Count > 0)
            {
                foreach (DC_Stg_Kafka KafkaData in objUnprocessedData)
                {
                    if (KafkaData.Error.ToUpper() == "SUCCESS")
                    {
                        Result = ProcessKafkaPayload(KafkaData);
                    }

                    UpdateStg_KafkaInfo(KafkaData);
                }
            }

            return Result;
        }

        public static List<DC_Stg_Kafka> GetAllUnprocessedData()
        {
            List<DC_Stg_Kafka> returnObj = new List<DC_Stg_Kafka>();
            try
            {
                //get all records from stg_kafka which are unread
                returnObj = Proxy.Get<List<DC_Stg_Kafka>>(System.Configuration.ConfigurationManager.AppSettings["GetPoll_Data"]).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                returnObj = new List<DC_Stg_Kafka>();
            }

            return returnObj;
        }

        public static bool ProcessKafkaPayload(DC_Stg_Kafka KafkaData)
        {
            bool Result = false;

            try
            {
                string row_id = KafkaData.Row_Id.ToString();
                string payload = KafkaData.PayLoad;
                string topic = KafkaData.Topic;

                JObject rss = JObject.Parse(payload);
                string method = (string)rss["method"];
                JObject data = (JObject)rss["data"];

                if ((method.ToUpper() == "PUT" || method.ToUpper() == "POST") && topic.ToUpper().EndsWith(".PRODUCTACCO.PUB"))
                {
                    AccommodationPayload AccoPayload = JsonConvert.DeserializeObject<AccommodationPayload>(data.ToString());

                    if (AccoPayload != null)
                    {
                        Result = Process_AccommodationData(AccoPayload, KafkaData);
                    }
                }
                else if (method.ToUpper() == "DELETE" && topic.ToUpper().EndsWith(".PRODUCTACCO.PUB"))
                {
                    Result = DeleteMasterRoom(data.ToString(), Guid.Empty);
                }

                return Result;

            }
            catch (Exception ex)
            {
                return Result;
            }
        }

        public static bool Process_AccommodationData(AccommodationPayload AccoData, DC_Stg_Kafka dC_Stg_Kafka)
        {
            if (AccoData.accomodationData != null)
            {
                Guid AccommodationId = Guid.Empty;
                DC_Accomodation dbAcco = new DC_Accomodation();

                var resAddUpdateAccommodationData = AddUpdateAccommodationData(AccoData);

                dbAcco = resAddUpdateAccommodationData.Item1;
                bool IsUpdate = resAddUpdateAccommodationData.Item2;
                if (dbAcco != null)
                {

                    AccommodationId = dbAcco.Accommodation_Id;

                    UpdateStg_KafkaInfoWithLog(dC_Stg_Kafka, "Accommodation is Loaded.", AccommodationId);

                    if (IsUpdate)
                    {
                        DeleteHotelContacts(AccommodationId);
                        DeleteHotelStatus(AccommodationId);
                        DeleteHoteldescription(AccommodationId);
                        DeleteHotelFacilities(AccommodationId);
                    }

                    AddHotelContacts(dbAcco, AccoData);

                    if (AccoData.accomodationRoomData != null && AccoData.accomodationRoomData.Count > 0)
                    {
                        bool isDataLoad = ProcessAccoRoomData(dbAcco, AccoData.accomodationRoomData);

                        if (isDataLoad)
                        {
                            UpdateStg_KafkaInfoWithLog(dC_Stg_Kafka, "RoomData is Loaded.", AccommodationId);
                        }
                    }

                    if (AccoData.accomodationData.productStatus != null)
                    {
                        bool isDataLoad = AddAccomodationStatusList(dbAcco, AccoData);
                        if (isDataLoad)
                        {
                            UpdateStg_KafkaInfoWithLog(dC_Stg_Kafka, "Accomodation Status are Loaded.", AccommodationId);
                        }
                    }

                    if (AccoData.accomodationData.facility != null && AccoData.accomodationData.facility.Count > 0)
                    {
                        bool isDataLoad = AddAccomodationFacilitiesList(dbAcco, AccoData);
                        if (isDataLoad)
                        {
                            UpdateStg_KafkaInfoWithLog(dC_Stg_Kafka, "Accomodation Facilities are Loaded.", AccommodationId);
                        }
                    }

                    if (AccoData.accomodationData.accomodationInfo.general.extras != null && AccoData.accomodationData.accomodationInfo.general.extras.Count() > 0)
                    {
                        bool isDataLoad = AddHotelDescriptionList(dbAcco, AccoData);
                        if (isDataLoad)
                        {
                            UpdateStg_KafkaInfoWithLog(dC_Stg_Kafka, "Accomodation HotelDescription are Loaded.", AccommodationId);
                        }
                    }
                }
            }

            return true;
        }

        #region Accommodation
        public static Tuple<DC_Accomodation, bool> AddUpdateAccommodationData(AccommodationPayload AccoData)
        {

            if (string.IsNullOrEmpty(AccoData.accomodationData.accomodationInfo.companyId))
            {
                return new Tuple<DC_Accomodation, bool>(null, false);
            }


            Guid AccommodationId = Guid.Empty;
            bool IsUpdate = false;

            string TelephoneTX = string.Empty;

            var acco = AccoData.accomodationData;

            #region Check if the hotel exists or not
            if (int.TryParse(acco.Legacy_Htl_Id, out int LegacyHotelId))
            {
                var SearchAccommodation = Proxy.Post<List<DC_Accomodation_Search_RS>, DC_Accomodation_Search_RQ>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_SearchURI"], new DC_Accomodation_Search_RQ
                {
                    CompanyHotelId = LegacyHotelId,
                    PageNo = 0,
                    PageSize = 1
                }).GetAwaiter().GetResult();

                if (SearchAccommodation != null && SearchAccommodation.Count > 0)
                {
                    AccommodationId = Guid.Parse(SearchAccommodation[0].AccomodationId);
                    IsUpdate = true;
                }
            }
            else
            {
                var SearchAccommodation = Proxy.Post<List<DC_Accomodation_Search_RS>, DC_Accomodation_Search_RQ>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_SearchURI"], new DC_Accomodation_Search_RQ
                {
                    TLGXAccoId = acco._id,
                    PageNo = 0,
                    PageSize = 1
                }).GetAwaiter().GetResult();

                if (SearchAccommodation != null && SearchAccommodation.Count > 0)
                {
                    AccommodationId = Guid.Parse(SearchAccommodation[0].AccomodationId);
                    IsUpdate = true;
                }
            }
            #endregion

            #region generate telephone tx
            if (acco.accomodationInfo.contactDetails != null && acco.accomodationInfo.contactDetails.Count > 0)
            {
                if (acco.accomodationInfo.contactDetails[0].phone != null)
                {
                    TelephoneTX = CommonFunctions.GetDigits((acco.accomodationInfo.contactDetails[0].phone.countryCode + acco.accomodationInfo.contactDetails[0].phone.cityCode + acco.accomodationInfo.contactDetails[0].phone.number), 8);
                }
            }
            #endregion

            #region Generate new acco if if acco not found
            if (AccommodationId == Guid.Empty) //Update
            {
                AccommodationId = Guid.NewGuid();
                IsUpdate = false;
            }
            #endregion

            #region Construct acco details to Add / Delete

            DC_Accomodation accoToInsertUpdate = new DC_Accomodation();

            accoToInsertUpdate.Accommodation_Id = AccommodationId;
            accoToInsertUpdate.Legacy_HTL_ID = LegacyHotelId;

            if (acco.createdAt != null && acco.createdAt <= DateTime.MinValue)
            {
                accoToInsertUpdate.Create_Date = null;
            }
            else
            {
                accoToInsertUpdate.Create_Date = acco.createdAt;
            }

            accoToInsertUpdate.Affiliation = acco.accomodationInfo.affiliations;
            accoToInsertUpdate.Area = acco.accomodationInfo.address.area;
            accoToInsertUpdate.AwardsReceived = acco.accomodationInfo.general.awardsReceived;
            accoToInsertUpdate.Brand = acco.accomodationInfo.brand;
            accoToInsertUpdate.Chain = acco.accomodationInfo.chain;
            accoToInsertUpdate.CheckInTime = acco.accomodationInfo.checkInTime;
            accoToInsertUpdate.CheckOutTime = acco.accomodationInfo.checkOutTime;
            accoToInsertUpdate.City = acco.accomodationInfo.address.city;
            accoToInsertUpdate.CompanyName = acco.accomodationInfo.companyName;
            accoToInsertUpdate.CompanyRating = acco.accomodationInfo.companyRating;
            accoToInsertUpdate.CompanyRecommended = acco.overview.isCompanyRecommended;
            accoToInsertUpdate.Country = acco.accomodationInfo.address.country;
            accoToInsertUpdate.Create_User = acco.createdBy ?? "Kafka_Sync";
            accoToInsertUpdate.DisplayName = acco.accomodationInfo.displayName;
            accoToInsertUpdate.Interest = (acco.overview != null && acco.overview.interest != null && acco.overview.interest.Count > 0 ? string.Join(",", acco.overview.interest) : null);
            if (acco.lastUpdated != null && acco.lastUpdated <= DateTime.MinValue)
            {
                accoToInsertUpdate.Edit_Date = null;
            }
            else
            {
                accoToInsertUpdate.Edit_Date = acco.lastUpdated;
            }

            accoToInsertUpdate.Edit_User = acco.updatedBy ?? "Kafka_Sync";

            if (int.TryParse(acco.accomodationInfo.financeControlId, out int financeControlId))
            {
                accoToInsertUpdate.FinanceControlID = financeControlId;
            }

            accoToInsertUpdate.Hashtag = string.Join(",", acco.overview.hashTag);
            accoToInsertUpdate.HotelName = acco.accomodationInfo.name;
            accoToInsertUpdate.HotelRating = acco.accomodationInfo.rating;
            accoToInsertUpdate.InsertFrom = true;
            accoToInsertUpdate.InternalRemarks = acco.accomodationInfo.general.internalRemarks;
            accoToInsertUpdate.IsActive = !acco.deleted;
            accoToInsertUpdate.IsMysteryProduct = acco.accomodationInfo.isMysteryProduct;

            //Accommodation Version Data
            accoToInsertUpdate.AccVersion = new DC_Accommodation_CompanyVersion();
            accoToInsertUpdate.AccVersion.Accommodation_Id = AccommodationId;
            accoToInsertUpdate.AccVersion.CompanyId = acco.accomodationInfo.companyId;
            accoToInsertUpdate.AccVersion.CommonProductId = acco.accomodationInfo.commonProductId;
            accoToInsertUpdate.AccVersion.CompanyProductId = acco.accomodationInfo.companyProductId;
            accoToInsertUpdate.AccVersion.CompanyName = acco.accomodationInfo.companyName;
            accoToInsertUpdate.AccVersion.ProductName = acco.accomodationInfo.name;
            accoToInsertUpdate.AccVersion.ProductDisplayName = acco.accomodationInfo.displayName;//Check
            accoToInsertUpdate.AccVersion.StarRating = acco.accomodationInfo.rating;
            accoToInsertUpdate.AccVersion.CompanyRating = acco.accomodationInfo.companyRating;
            accoToInsertUpdate.AccVersion.ProductCatSubType = acco.accomodationInfo.productCatSubType;
            accoToInsertUpdate.AccVersion.Brand = acco.accomodationInfo.brand;
            accoToInsertUpdate.AccVersion.Chain = acco.accomodationInfo.chain;
            //Need to confirm Address
            accoToInsertUpdate.AccVersion.HouseNumber = acco.accomodationInfo.address.houseNumber;
            accoToInsertUpdate.AccVersion.Street = acco.accomodationInfo.address.street;
            accoToInsertUpdate.AccVersion.Street2 = acco.accomodationInfo.address.street2;
            accoToInsertUpdate.AccVersion.Street3 = acco.accomodationInfo.address.street3;
            accoToInsertUpdate.AccVersion.Street4 = acco.accomodationInfo.address.street4;
            accoToInsertUpdate.AccVersion.Street5 = acco.accomodationInfo.address.street5;
            accoToInsertUpdate.AccVersion.Zone = acco.accomodationInfo.address.zone;
            accoToInsertUpdate.AccVersion.PostalCode = acco.accomodationInfo.address.postalCode;
            accoToInsertUpdate.AccVersion.Country = acco.accomodationInfo.address.country;
            accoToInsertUpdate.AccVersion.State = acco.accomodationInfo.address.state;
            accoToInsertUpdate.AccVersion.City = acco.accomodationInfo.address.city;
            accoToInsertUpdate.AccVersion.Area = acco.accomodationInfo.address.area;
            accoToInsertUpdate.AccVersion.Location = acco.accomodationInfo.address.location;
            accoToInsertUpdate.AccVersion.TLGXAccoId = acco._id;
            accoToInsertUpdate.AccVersion.Interest = (acco.overview != null && acco.overview.interest != null && acco.overview.interest.Count > 0 ? string.Join(",", acco.overview.interest) : null);


            if (acco.accomodationInfo.address.geometry.coordinates != null && acco.accomodationInfo.address.geometry.coordinates.Count > 0)
            {
                accoToInsertUpdate.AccVersion.Latitude = acco.accomodationInfo.address.geometry.coordinates[0].ToString();
                accoToInsertUpdate.AccVersion.Longitude = acco.accomodationInfo.address.geometry.coordinates[1].ToString();
            }


            if (acco.accomodationInfo.address.geometry.coordinates != null && acco.accomodationInfo.address.geometry.coordinates.Count > 0)
            {
                accoToInsertUpdate.Latitude = acco.accomodationInfo.address.geometry.coordinates[0].ToString();
                accoToInsertUpdate.Longitude = acco.accomodationInfo.address.geometry.coordinates[1].ToString();
            }

            accoToInsertUpdate.Location = acco.accomodationInfo.address.location;

            if (acco.productStatus.from != null)
            {
                if (acco.productStatus.from.year != 0 && acco.productStatus.from.month != 0 && acco.productStatus.from.day != 0)
                {
                    accoToInsertUpdate.OfflineDate = new DateTime(acco.productStatus.from.year, acco.productStatus.from.month, acco.productStatus.from.day);
                }
            }

            if (acco.productStatus.to != null)
            {
                if (acco.productStatus.to.year != 0 && acco.productStatus.to.month != 0 && acco.productStatus.to.day != 0)
                {
                    accoToInsertUpdate.OnlineDate = new DateTime(acco.productStatus.to.year, acco.productStatus.to.month, acco.productStatus.to.day);
                }
            }

            accoToInsertUpdate.PostalCode = acco.accomodationInfo.address.postalCode;
            accoToInsertUpdate.ProductCategorySubType = acco.accomodationInfo.productCatSubType;

            if (acco.accomodationInfo.ratingDatedOn != null)
            {
                if (acco.accomodationInfo.ratingDatedOn.year != 0 && acco.accomodationInfo.ratingDatedOn.month != 0 && acco.accomodationInfo.ratingDatedOn.day != 0)
                {
                    accoToInsertUpdate.RatingDate = new DateTime(acco.accomodationInfo.ratingDatedOn.year, acco.accomodationInfo.ratingDatedOn.month, acco.accomodationInfo.ratingDatedOn.day);
                }
            }

            accoToInsertUpdate.Reason = acco.productStatus.reason;
            accoToInsertUpdate.RecommendedFor = string.Join(",", acco.accomodationInfo.recommendedFor);
            accoToInsertUpdate.Remarks = acco.productStatus.remark;
            accoToInsertUpdate.State_Name = acco.accomodationInfo.address.state;
            accoToInsertUpdate.StreetName = acco.accomodationInfo.address.street;
            accoToInsertUpdate.StreetNumber = acco.accomodationInfo.address.houseNumber;
            accoToInsertUpdate.Town = acco.accomodationInfo.address.street2;
            accoToInsertUpdate.Street3 = acco.accomodationInfo.address.street3;
            accoToInsertUpdate.Street4 = acco.accomodationInfo.address.street4;
            accoToInsertUpdate.Street5 = acco.accomodationInfo.address.street5;
            accoToInsertUpdate.SuburbDowntown = acco.accomodationInfo.address.zone;
            accoToInsertUpdate.TLGXAccoId = acco._id;
            accoToInsertUpdate.TotalFloors = acco.accomodationInfo.noOfFloors.ToString();
            accoToInsertUpdate.TotalRooms = acco.accomodationInfo.noOfRooms.ToString();
            accoToInsertUpdate.YearBuilt = acco.accomodationInfo.general.yearBuilt;
            accoToInsertUpdate.Telephone_TX = TelephoneTX;
            accoToInsertUpdate.ProductCategory = "Accommodation";
            #endregion

            #region Update / Add Accommodation

            var addUpdateAcco = Proxy.Post<bool, DC_Accomodation>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateURI"], accoToInsertUpdate).GetAwaiter().GetResult();
            #endregion

            return new Tuple<DC_Accomodation, bool>(accoToInsertUpdate, IsUpdate);
        }

        public static List<DC_Accommodation_CompanyVersion> GetAccomodationCompanyVersionInfo(Guid Acco_id)
        {
            List<DC_Accommodation_CompanyVersion> AccoList = new List<DC_Accommodation_CompanyVersion>();
            AccoList = Proxy.Get<List<DC_Accommodation_CompanyVersion>>(string.Format(System.Configuration.ConfigurationManager.AppSettings["AccomodationCompanyVersion_SearchURI"], Acco_id)).GetAwaiter().GetResult();
            return AccoList;
        }
        #endregion

        #region Description
        /// <summary>
        /// AddHotelDescriptionList does Same like AddHotelDescription
        /// But it Approches as Send Batch Instead of Single.
        /// </summary>
        /// <param name="Acco_id"></param>
        /// <param name="HotelData"></param>
        /// <param name="dt"></param>
        public static bool AddHotelDescriptionList(DC_Accomodation dbAcco, AccommodationPayload AccoData)
        {
            bool result = false;

            if (AccoData.accomodationData.accomodationInfo.general.extras.Count > 0)
            {
                List<MDMSVC.DC_Accommodation_Descriptions> newLst = new List<MDMSVC.DC_Accommodation_Descriptions>();
                foreach (var item in AccoData.accomodationData.accomodationInfo.general.extras)
                {
                    newLst.Add(new MDMSVC.DC_Accommodation_Descriptions()
                    {
                        Accommodation_Description_Id = Guid.NewGuid(),
                        Accommodation_Id = dbAcco.Accommodation_Id,
                        Legacy_Htl_Id = dbAcco.CompanyHotelID,
                        Description = item.description,
                        DescriptionType = item.label,
                        Create_Date = dbAcco.Create_Date,
                        Create_User = dbAcco.Create_User,
                        Edit_Date = dbAcco.Edit_Date,
                        Edit_User = dbAcco.Edit_User,
                        IsActive = true
                    });
                }

                if (newLst.Count > 0)
                {
                    result = Proxy.Post<bool, List<DC_Accommodation_Descriptions>>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddLstDescriptionURI"], newLst).GetAwaiter().GetResult();
                }
                newLst = null;
            }

            return result;
        }

        public static DC_Message DeleteHoteldescription(Guid AccommodationId)
        {
            return Proxy.Post<DC_Message, DC_Accommodation_Descriptions>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteDescriptionURI"], new DC_Accommodation_Descriptions
            {
                Accommodation_Id = AccommodationId
            }).GetAwaiter().GetResult();
        }

        #endregion description

        #region Facility
        public static bool AddAccomodationFacilitiesList(DC_Accomodation dbAcco, AccommodationPayload AccoData)
        {
            bool Result = false;
            List<string> DistinctValues = new List<string>();
            List<DC_Accommodation_Facility> newLst = new List<DC_Accommodation_Facility>();

            if (AccoData.accomodationData.facility != null && AccoData.accomodationData.facility.Count > 0)
            {
                foreach (var item in AccoData.accomodationData.facility)
                {
                    DC_Accommodation_Facility AF = new DC_Accommodation_Facility();

                    //////// ****************Below Code is for Eliminating Duplicate Entries basis of Category, Type and Description*********************
                    if (DistinctValues.FindIndex(s => s.Contains(Convert.ToString(item.category) + Convert.ToString(item.type) + Convert.ToString(item.desc))) <= -1)
                    {
                        DistinctValues.Add(Convert.ToString(item.category) + Convert.ToString(item.type) + Convert.ToString(item.desc));
                        AF.Accommodation_Facility_Id = Guid.NewGuid();
                        AF.Accommodation_Id = dbAcco.Accommodation_Id;
                        AF.Legacy_Htl_Id = dbAcco.CompanyHotelID;
                        AF.FacilityCategory = item.category;
                        AF.FacilityType = item.type;
                        AF.FacilityName = null;
                        AF.Description = item.desc;
                        AF.Create_Date = dbAcco.Create_Date;
                        AF.Create_User = dbAcco.Create_User;
                        AF.Edit_Date = dbAcco.Edit_Date;
                        AF.Edit_User = dbAcco.Edit_User;
                        AF.IsActive = true;
                        newLst.Add(AF);
                    }

                    //////// ****************Below Commented Code is for Accepting Duplicate Entries basis of Category, Type and Description*********************
                    //DistinctValues.Add(Convert.ToString(item.category) + Convert.ToString(item.type) + Convert.ToString(item.desc));
                    //AF.Accommodation_Facility_Id = Guid.NewGuid(); AF.Accommodation_Id = new Guid(Acco_id);
                    //AF.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId); AF.FacilityCategory = item.category;
                    //AF.FacilityType = item.type; AF.FacilityName = null; AF.Description = item.desc; AF.Create_Date = DateTime.Now; AF.Create_User = "Kafka";
                    //AF.Edit_Date = DateTime.Now; AF.Edit_User = "Kafka"; AF.IsActive = true; newLst.Add(AF);

                }

                if (newLst.Count > 0)
                {
                    Result = Proxy.Post<bool, List<DC_Accommodation_Facility>>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddLstFacilitiesURI"], newLst).GetAwaiter().GetResult();
                }
                newLst = null;
            }

            return Result;
        }

        public static DC_Message DeleteHotelFacilities(Guid AccommodationId)
        {
            return Proxy.Post<DC_Message, DC_Accommodation_Facility>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteFacilitiesURI"], new DC_Accommodation_Facility
            {
                Accommodation_Id = AccommodationId
            }).GetAwaiter().GetResult();
        }
        #endregion

        #region Contact
        public static void AddHotelContacts(DC_Accomodation acco, AccommodationPayload AccoData)
        {
            #region Generate All contact details
            List<DC_Accommodation_Contact> accoContacts = new List<DC_Accommodation_Contact>();
            foreach (var contact in AccoData.accomodationData.accomodationInfo.contactDetails)
            {
                accoContacts.Add(new DC_Accommodation_Contact
                {
                    Accommodation_Contact_Id = Guid.NewGuid(),
                    Accommodation_Id = acco.Accommodation_Id,
                    Create_Date = acco.Create_Date ?? DateTime.Now,
                    Create_User = acco.Create_User,
                    Edit_Date = acco.Edit_Date ?? DateTime.Now,
                    Edit_User = acco.Edit_User,
                    Email = contact.emailAddress,
                    Legacy_Htl_Id = acco.Legacy_HTL_ID,
                    Telephone = contact.phone == null ? string.Empty : contact.phone.countryCode + "-" + contact.phone.cityCode + "-" + contact.phone.number,
                    Fax = contact.fax == null ? string.Empty : contact.fax.countryCode + "-" + contact.fax.cityCode + "-" + contact.fax.number,
                    WebSiteURL = contact.website,
                    City_Id = acco.City_Id,
                    Country_Id = acco.Country_Id,
                    IsActive = true
                });
            }
            #endregion

            #region Add Acco Contacts
            var addContacts = Proxy.Post<bool, List<DC_Accommodation_Contact>>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddContactsURI"], accoContacts).GetAwaiter().GetResult();
            #endregion
        }

        public static void DeleteHotelContacts(Guid AccommodationId)
        {
            if (AccommodationId != Guid.Empty)
            {
                //Delete all Contact details
                var deleteContacts = Proxy.Post<DC_Message, DC_Accommodation_Contact>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteContactURI"], new DC_Accommodation_Contact
                {
                    Accommodation_Id = AccommodationId
                }).GetAwaiter().GetResult();
            }
        }

        #endregion contact

        #region Status
        public static bool AddAccomodationStatusList(DC_Accomodation dbAcco, AccommodationPayload AccoData)
        {
            try
            {
                List<DC_Accommodation_Status> Lst = new List<DC_Accommodation_Status>();

                if (AccoData.accomodationData.productStatus.deactivated.Count > 0)
                {
                    foreach (var item in AccoData.accomodationData.productStatus.deactivated)
                    {
                        DC_Accommodation_Status AC = new DC_Accommodation_Status();
                        {
                            AC.Accommodation_Status_Id = Guid.NewGuid();
                            AC.Accommodation_Id = dbAcco.Accommodation_Id;
                            AC.From = (item.@from != null ? DateTime.Parse((item.@from).year + "-" + (item.@from).month + "-" + item.@from.day) :
                                (AccoData.accomodationData.productStatus.@from != null ?
                                DateTime.Parse((AccoData.accomodationData.productStatus.@from).year + "-" +
                                (AccoData.accomodationData.productStatus.@from).month + "-" + AccoData.accomodationData.productStatus.@from.day) :
                                (DateTime?)null));

                            AC.To = (item.to != null ? DateTime.Parse((item.to).year + "-" + (item.to).month + "-" + item.to.day) :
                               (AccoData.accomodationData.productStatus.@to != null ?
                               DateTime.Parse((AccoData.accomodationData.productStatus.@to).year + "-" +
                               (AccoData.accomodationData.productStatus.@to).month + "-" + AccoData.accomodationData.productStatus.@to.day) :
                               (DateTime?)null));

                            AC.DeactivationReason = (item.@from != null ? item.reason : null);
                            AC.Status = AccoData.accomodationData.productStatus.status;
                            AC.CompanyMarket = (item.@from != null ? item.marketName : null);
                            AC.IsActive = true;
                            AC.Create_Date = dbAcco.Create_Date;
                            AC.Create_User = dbAcco.Create_User;
                            AC.Edit_Date = dbAcco.Edit_Date;
                            AC.Edit_User = dbAcco.Edit_User;

                            Lst.Add(AC);
                        }
                    }
                }
                else
                {

                    DC_Accommodation_Status AC = new DC_Accommodation_Status();
                    {
                        AC.Accommodation_Status_Id = Guid.NewGuid();
                        AC.Accommodation_Id = dbAcco.Accommodation_Id;
                        AC.From = (AccoData.accomodationData.productStatus.@from != null ?
                            DateTime.Parse((AccoData.accomodationData.productStatus.@from).year + "-" +
                            (AccoData.accomodationData.productStatus.@from).month + "-" + AccoData.accomodationData.productStatus.@from.day) :
                            (DateTime?)null);

                        AC.To = (AccoData.accomodationData.productStatus.@to != null ?
                           DateTime.Parse((AccoData.accomodationData.productStatus.@to).year + "-" +
                           (AccoData.accomodationData.productStatus.@to).month + "-" + AccoData.accomodationData.productStatus.@to.day) :
                           (DateTime?)null);

                        AC.DeactivationReason = null;
                        AC.Status = AccoData.accomodationData.productStatus.status;
                        AC.CompanyMarket = null;
                        AC.IsActive = true;
                        AC.Create_Date = dbAcco.Create_Date;
                        AC.Create_User = dbAcco.Create_User;
                        AC.Edit_Date = dbAcco.Edit_Date;
                        AC.Edit_User = dbAcco.Edit_User;

                        Lst.Add(AC);
                    }

                }

                var result = Proxy.Post<bool, List<DC_Accommodation_Status>>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddLstStatusURI"], Lst).GetAwaiter().GetResult();
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DC_Message DeleteHotelStatus(Guid AccommodationId)
        {
            return Proxy.Post<DC_Message, DC_Accommodation_Status>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteStatusURI"], new DC_Accommodation_Status
            {
                Accommodation_Id = AccommodationId
            }).GetAwaiter().GetResult();
        }
        #endregion status

        #region Room

        public static bool ProcessAccoRoomData(DC_Accomodation dbAcco, List<AccomodationRoomData> AccoRoomData)
        {

            List<DC_Accommodation_RoomInfo> ExistingRooms = GetMasterRoomList(dbAcco.Accommodation_Id);
            var result = AddUpdateAccoRooms(ExistingRooms, dbAcco, AccoRoomData);
            return result;
        }

        //Room Id cannot be null as per new json response from Kafka
        public static List<DC_Accommodation_RoomInfo> GetMasterRoomList(Guid Acco_id)
        {
            List<DC_Accommodation_RoomInfo> RoomList = new List<DC_Accommodation_RoomInfo>();
            RoomList = Proxy.Get<List<DC_Accommodation_RoomInfo>>(string.Format(System.Configuration.ConfigurationManager.AppSettings["Accomodation_RoomSearch"], Acco_id, Guid.Empty)).GetAwaiter().GetResult();
            return RoomList;
        }

        public static bool AddUpdateAccoRooms(List<DC_Accommodation_RoomInfo> ExistingRooms, DC_Accomodation dbAcco, List<AccomodationRoomData> AccoRoomData)
        {
            try
            {
                List<Guid> NewRooms = new List<Guid>();
                bool IsUpdate = false;
                List<DC_Accommodation_CompanyVersion> lstAccommodation_CompanyVersion = GetAccomodationCompanyVersionInfo(dbAcco.Accommodation_Id);

                foreach (var room in AccoRoomData)
                {


                    if (!string.IsNullOrEmpty(room.commonRoomId))
                    {
                        //Check if room is there
                        var ExistingAccommodationRoom = ExistingRooms.Where(w => w.TLGXAccoRoomId.ToUpper() == room._id.ToUpper() && w.CommonRoomId == (room.commonRoomId == null ? string.Empty : room.commonRoomId)).FirstOrDefault();


                        DC_Accommodation_RoomInfo RoomToAddUpdate = new DC_Accommodation_RoomInfo
                        {
                            Accommodation_Id = dbAcco.Accommodation_Id,
                            Accommodation_RoomInfo_Id = ExistingAccommodationRoom == null ? Guid.NewGuid() : ExistingAccommodationRoom.Accommodation_RoomInfo_Id,
                            Legacy_Htl_Id = dbAcco.CompanyHotelID,
                            BathRoomType = room.bathroomType == null ? (ExistingAccommodationRoom == null ? room.bathroomType : ExistingAccommodationRoom.BathRoomType) : room.bathroomType,
                            BedType = room.bedType == null ? (ExistingAccommodationRoom == null ? room.bedType : ExistingAccommodationRoom.BedType) : room.bedType,
                            Category = room.category == null ? (ExistingAccommodationRoom == null ? room.category : ExistingAccommodationRoom.Category) : room.category,
                            CompanyRoomCategory = room.companyRoomCategory == null ? (ExistingAccommodationRoom == null ? room.companyRoomCategory : ExistingAccommodationRoom.CompanyRoomCategory) : room.companyRoomCategory,
                            //Edit_Date = room.lastUpdated,
                            Edit_User = dbAcco.Edit_User,
                            //Create_Date = room.createdAt,
                            Create_User = dbAcco.Create_User,
                            FloorName = room.floorName == null ? (ExistingAccommodationRoom == null ? room.floorName : ExistingAccommodationRoom.FloorName) : room.floorName,
                            FloorNumber = room.floorNo == 0 ? (ExistingAccommodationRoom == null ? room.floorNo.ToString() : ExistingAccommodationRoom.FloorNumber) : room.floorNo.ToString(),
                            RoomCategory = room.category == null ? (ExistingAccommodationRoom == null ? room.category : ExistingAccommodationRoom.RoomCategory) : room.category,
                            RoomDecor = room.roomDecor == null ? (ExistingAccommodationRoom == null ? room.roomDecor : ExistingAccommodationRoom.RoomDecor) : room.roomDecor,
                            RoomName = room.name == null ? (ExistingAccommodationRoom == null ? room.name : ExistingAccommodationRoom.RoomName) : room.name,
                            RoomSize = room.roomSize == 0 ? (ExistingAccommodationRoom == null ? room.roomSize.ToString() : ExistingAccommodationRoom.RoomSize) : room.roomSize.ToString(),
                            RoomView = room.view == null ? (ExistingAccommodationRoom == null ? room.view : ExistingAccommodationRoom.RoomView) : room.view,
                            Smoking = ExistingAccommodationRoom == null ? null : ExistingAccommodationRoom.Smoking,
                            Description = room.roomDescription == null ? (ExistingAccommodationRoom == null ? room.roomDescription : ExistingAccommodationRoom.Description) : room.roomDescription,
                            IsActive = !room.deleted,
                            RoomId = room.roomId == null ? (ExistingAccommodationRoom == null ? room.roomId : ExistingAccommodationRoom.RoomId) : room.roomId,
                            TLGXAccoRoomId = room._id,
                            MysteryRoom = room.isMysteryRoom,
                            NoOfInterconnectingRooms = room.noOfInterconnectingRooms == 0 ? (ExistingAccommodationRoom == null ? room.noOfInterconnectingRooms : ExistingAccommodationRoom.NoOfInterconnectingRooms) : room.noOfInterconnectingRooms,
                            NoOfRooms = room.noOfRooms == 0 ? (ExistingAccommodationRoom == null ? room.noOfRooms : ExistingAccommodationRoom.NoOfRooms) : room.noOfRooms,
                            CompanyName = ExistingAccommodationRoom == null ? null : ExistingAccommodationRoom.CompanyName,
                            IsAmenityChanges = ExistingAccommodationRoom == null ? false : ExistingAccommodationRoom.IsAmenityChanges,
                            AmenityTypes = ExistingAccommodationRoom == null ? null : ExistingAccommodationRoom.AmenityTypes,
                            CommonRoomId = ExistingAccommodationRoom == null ? null : ExistingAccommodationRoom.CommonRoomId,




                        };

                        if (room.createdAt != null && room.createdAt <= DateTime.MinValue)
                        {
                            RoomToAddUpdate.Create_Date = null;
                        }
                        else
                        {
                            RoomToAddUpdate.Create_Date = room.createdAt;
                        }


                        if (room.lastUpdated != null && room.lastUpdated <= DateTime.MinValue)
                        {
                            RoomToAddUpdate.Create_Date = null;
                        }
                        else
                        {
                            RoomToAddUpdate.Edit_Date = room.lastUpdated;
                        }

                        DC_Accommodation_CompanyVersion companyVersion = lstAccommodation_CompanyVersion.Where(x => x.Accommodation_Id == dbAcco.Accommodation_Id && x.CommonProductId == dbAcco.AccVersion.CommonProductId && x.CompanyId == dbAcco.AccVersion.CompanyId).SingleOrDefault();

                        RoomToAddUpdate.AccoRoomVersion = new Accommodation_RoomInfo_CompanyVersion();
                        RoomToAddUpdate.AccoRoomVersion.Accommodation_CompanyVersion_Id = companyVersion.Accommodation_CompanyVersion_Id;
                        RoomToAddUpdate.AccoRoomVersion.BedType = RoomToAddUpdate.BedType;
                        RoomToAddUpdate.AccoRoomVersion.Accommodation_RoomInfo_Id = RoomToAddUpdate.Accommodation_RoomInfo_Id;
                        RoomToAddUpdate.AccoRoomVersion.RoomCategory = RoomToAddUpdate.RoomCategory;
                        RoomToAddUpdate.AccoRoomVersion.RoomName = RoomToAddUpdate.RoomName;
                        RoomToAddUpdate.AccoRoomVersion.CompanyRoomCategory = RoomToAddUpdate.CompanyRoomCategory;
                        RoomToAddUpdate.AccoRoomVersion.RoomDescription = room.roomDescription;
                        RoomToAddUpdate.AccoRoomVersion.TlgxAccoId = companyVersion.TLGXAccoId;
                        RoomToAddUpdate.AccoRoomVersion.TlgxAccoRoomId = room._id;


                        if (ExistingAccommodationRoom == null)
                        {
                            var AddRoomResult = Proxy.Post<bool, DC_Accommodation_RoomInfo>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddRoomURI"], RoomToAddUpdate).GetAwaiter().GetResult();
                            //Task.WaitAny(Proxy.Post<bool, DC_Accommodation_RoomInfo>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddRoomURI"], RoomToAddUpdate));

                            IsUpdate = AddRoomResult;

                        }
                        else
                        {
                            var UpdateRoomResult = Proxy.Post<bool, DC_Accommodation_RoomInfo>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateRoomURI"], RoomToAddUpdate).GetAwaiter().GetResult();

                            IsUpdate = UpdateRoomResult;

                            //Remove Room Facilities
                            if (room.amenities != null && room.amenities.Count() > 0)
                            {
                                var RemoveRoomFacilityResult = Proxy.Get<bool>(string.Format(System.Configuration.ConfigurationManager.AppSettings["Accommodation_DeleteRoomFacilities_ByAccoRoomId"], ExistingAccommodationRoom.Accommodation_RoomInfo_Id)).GetAwaiter().GetResult();
                            }
                        }

                        NewRooms.Add(RoomToAddUpdate.Accommodation_RoomInfo_Id);

                        //Add Room Amenities
                        if (room.amenities != null && room.amenities.Count() > 0)
                        {
                            foreach (var roomAmenity in room.amenities)
                            {
                                var roomFacilites = new DC_Accomodation_RoomFacilities
                                {
                                    Accommodation_Id = dbAcco.Accommodation_Id,
                                    Accommodation_RoomInfo_Id = RoomToAddUpdate.Accommodation_RoomInfo_Id,
                                    Accommodation_RoomFacility_Id = Guid.NewGuid(),
                                    AmenityName = roomAmenity.name,
                                    AmenityType = roomAmenity.type,
                                    IsActive = true,
                                    IsRoomActive = true,

                                    Create_User = dbAcco.Create_User,
                                    Description = roomAmenity.desc,
                                    //Edit_Date = room.lastUpdated,
                                    Edit_user = dbAcco.Edit_User
                                };


                                if (room.createdAt != null && room.createdAt <= DateTime.MinValue)
                                {
                                    roomFacilites.Create_Date = null;
                                }
                                else
                                {
                                    roomFacilites.Create_Date = room.createdAt;
                                }


                                if (room.lastUpdated != null && room.lastUpdated <= DateTime.MinValue)
                                {
                                    roomFacilites.Create_Date = null;
                                }
                                else
                                {
                                    roomFacilites.Edit_Date = room.lastUpdated;
                                }




                                var AddRoomFacilityResult = Proxy.Post<bool, DC_Accomodation_RoomFacilities>(System.Configuration.ConfigurationManager.AppSettings["Accommodation_AddRoomFacilities"], roomFacilites).GetAwaiter().GetResult();
                            }
                        }

                    }
                }

                //Remove Unupdated Rooms
                var RoomsToInActivate = ExistingRooms.Where(w => !NewRooms.Contains(w.Accommodation_RoomInfo_Id)).Select(s => s.Accommodation_RoomInfo_Id).ToList();
                foreach (Guid RoomInfoId in RoomsToInActivate)
                {
                    DeleteMasterRoom(null, RoomInfoId);
                }

                return IsUpdate;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteMasterRoom(string AccoRoomId, Guid Accommodation_RoomInfo_Id)
        {

            DC_Accommodation_RoomInfo RoomToUpdate = new DC_Accommodation_RoomInfo
            {
                Edit_Date = DateTime.Now,
                Edit_User = "Kafka",
                IsActive = false,
                TLGXAccoRoomId = AccoRoomId,
                Accommodation_RoomInfo_Id = Accommodation_RoomInfo_Id
            };

            var UpdateRoomResult = Proxy.Post<bool, DC_Accommodation_RoomInfo>(System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateRoomURI"], RoomToUpdate).GetAwaiter().GetResult();

            return true;
        }

        #endregion Room

        public static async void UpdateStg_KafkaInfo(DC_Stg_Kafka Kafka)
        {
            DC_Stg_Kafka obj = new DC_Stg_Kafka();
            obj.Row_Id = Kafka.Row_Id;
            obj.Process_User = "Kafka";
            obj.Process_Date = DateTime.Now;
            obj.Status = "Processed";
            await Proxy.Post<DC_Message, DC_Stg_Kafka>(System.Configuration.ConfigurationManager.AppSettings["Kafka_Update"], obj);
            obj = null;
        }

        public static async void UpdateStg_KafkaInfoWithLog(DC_Stg_Kafka Kafka, string Message, Guid Accommodation_id)
        {
            DC_Stg_Kafka obj = new DC_Stg_Kafka();
            obj.Row_Id = Kafka.Row_Id;
            obj.Process_User = "Kafka";
            obj.message_log = Message;
            obj.Accommodation_Id = Accommodation_id;
            await Proxy.Post<DC_Message, DC_Stg_Kafka>(System.Configuration.ConfigurationManager.AppSettings["Kafka_Update"], obj);
            obj = null;
        }
    }
}
