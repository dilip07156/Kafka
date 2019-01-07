using KafkaConsumer.MDMSVC;
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
        private static readonly object RoomDataForUpdate;

        public static void InsertInto_StgKafka(Confluent.Kafka.Message<Confluent.Kafka.Null, string> msg)
        {
            MDMSVC.DC_Stg_Kafka obj = new MDMSVC.DC_Stg_Kafka()
            {
                Error = msg.Error.Reason,
                TopicPartion = msg.TopicPartition.Partition.ToString(),
                Key = Convert.ToString(msg.Key),
                TimeStamp = msg.Timestamp.UtcDateTime,
                PayLoad = msg.Value,
                Offset = msg.Offset.Value.ToString(),
                Partion = msg.Partition.ToString(),
                Create_User = "KafkaConsumer",
                Create_Date = DateTime.Now,
                Row_Id = Guid.NewGuid(),
                Topic = msg.Topic,
                TopicPartionOffset = msg.TopicPartition.Partition.ToString()
            };
            ProxyAsync.PostAsync(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Kafka_Insert"], obj, obj.GetType());
        }

        public static void UpdateStg_KafkaInfo(List<DC_Stg_Kafka> Kafka, string row_id)
        {
            MDMSVC.DC_Stg_Kafka obj = new MDMSVC.DC_Stg_Kafka();

            object result = null;
            obj.Row_Id = new Guid(row_id);
            //obj.Key = kafka.Key;
            obj.Offset = Kafka[0].Offset;
            // obj.TimeStamp = Kafka[0].TimeStamp;
            //obj.Partion = kafka[0].Partion;
            //obj.TopicPartion = kafka[0].TopicPartion;
            //obj.TopicPartionOffset = kafka[0].TopicPartionOffset;
            obj.Process_User = "Kafka";
            obj.Process_Date = DateTime.Now;
            obj.Status = "Read";
            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Kafka_Update"], obj, typeof(MDMSVC.DC_Stg_Kafka), typeof(bool), out result);
        }

        #region Description

        public static void AddHotelDescription(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            MDMSVC.DC_Accommodation_Descriptions newObj = new MDMSVC.DC_Accommodation_Descriptions();
            {
                object result = null;
                foreach (var item in dt.data.accomodationData.accomodationInfo.general.extras)
                {
                    newObj.Accommodation_Description_Id = Guid.NewGuid();
                    newObj.Accommodation_Id = Guid.Parse(Acco_id);
                    newObj.Description = item.description;
                    newObj.DescriptionType = "Short";
                    newObj.Create_Date = DateTime.Now;
                    newObj.Create_User = "Kafka";
                    newObj.Edit_Date = DateTime.Now;
                    newObj.Edit_User = "Kafka";
                    newObj.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                    newObj.IsActive = true;
                    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddDescriptionURI"], newObj, typeof(DC_Accommodation_Descriptions), typeof(bool), out result);
                }
            }
        }

        /// <summary>
        /// AddHotelDescriptionList does Same like AddHotelDescription
        /// But it Approches as Send Batch Instead of Single.
        /// </summary>
        /// <param name="Acco_id"></param>
        /// <param name="HotelData"></param>
        /// <param name="dt"></param>
        public static void AddHotelDescriptionList(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            object result = null;

            if (dt.data.accomodationData.accomodationInfo.general.extras.Count > 0)
            {
                List<MDMSVC.DC_Accommodation_Descriptions> newLst = new List<MDMSVC.DC_Accommodation_Descriptions>();
                {
                    foreach (var item in dt.data.accomodationData.accomodationInfo.general.extras)
                    {
                        MDMSVC.DC_Accommodation_Descriptions newObj1 = new MDMSVC.DC_Accommodation_Descriptions();
                        {
                            newObj1.Accommodation_Description_Id = Guid.NewGuid();
                            newObj1.Accommodation_Id = Guid.Parse(Acco_id);
                            newObj1.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                            newObj1.Description = item.description;
                            //newObj1.DescriptionType = "Short";
                            newObj1.DescriptionType = item.label;
                            newObj1.IsActive = true;
                            newObj1.Create_Date = DateTime.Now;
                            newObj1.Create_User = "Kafka";
                            newObj1.Edit_Date = DateTime.Now;
                            newObj1.Edit_User = "Kafka";
                            newLst.Add(newObj1);
                        }
                    }
                }

                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddLstDescriptionURI"], newLst, typeof(List<DC_Accommodation_Descriptions>), typeof(bool), out result);
            }
        }

        public static void DeleteHoteldescription(string Acco_id)
        {
            object result = null;
            MDMSVC.DC_Accommodation_Descriptions RQParams = new MDMSVC.DC_Accommodation_Descriptions();
            RQParams.Accommodation_Id = Guid.Parse(Acco_id);
            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteDescriptionURI"], RQParams, typeof(DC_Accommodation_Descriptions), typeof(DC_Message), out result);
        }

        #endregion description

        #region Facility
        public static void AddAccomodationFacilities(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            object result = null;
            MDMSVC.DC_Accommodation_Facility AF = new MDMSVC.DC_Accommodation_Facility();

            foreach (var item in dt.data.accomodationData.facility)
            {
                AF.Accommodation_Facility_Id = Guid.NewGuid();
                AF.Accommodation_Id = new Guid(Acco_id);
                AF.FacilityCategory = item.category;
                AF.FacilityType = item.type;
                AF.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                AF.Description = item.desc;
                AF.FacilityName = null;
                AF.Create_Date = DateTime.Now;
                AF.Create_User = "Kafka";
                AF.Edit_Date = DateTime.Now;
                AF.Edit_User = "Kafka";
                AF.IsActive = true;
                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddFacilitiesURI"], AF, typeof(DC_Accommodation_Facility), typeof(bool), out result);
            };

        }

        public static void AddAccomodationFacilitiesList(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            object result = null;
            List<string> DistinctValues = new List<string>();
            List<MDMSVC.DC_Accommodation_Facility> newLst = new List<MDMSVC.DC_Accommodation_Facility>();

            if (dt.data.accomodationData.facility.Count > 0)
            {
                foreach (var item in dt.data.accomodationData.facility)
                {
                    MDMSVC.DC_Accommodation_Facility AF = new MDMSVC.DC_Accommodation_Facility();

                    //////// ****************Below Code is for Eliminating Duplicate Entries basis of Category, Type and Description*********************
                    if (DistinctValues.FindIndex(s => s.Contains(Convert.ToString(item.category) + Convert.ToString(item.type) + Convert.ToString(item.desc))) <= -1)
                    {
                        DistinctValues.Add(Convert.ToString(item.category) + Convert.ToString(item.type) + Convert.ToString(item.desc));
                        AF.Accommodation_Facility_Id = Guid.NewGuid();
                        AF.Accommodation_Id = new Guid(Acco_id);
                        AF.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                        AF.FacilityCategory = item.category;
                        AF.FacilityType = item.type;
                        AF.FacilityName = null;
                        AF.Description = item.desc;
                        AF.Create_Date = DateTime.Now;
                        AF.Create_User = "Kafka";
                        AF.Edit_Date = DateTime.Now;
                        AF.Edit_User = "Kafka";
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

                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddLstFacilitiesURI"], newLst, typeof(List<DC_Accommodation_Facility>), typeof(bool), out result);

                newLst = null;
            }
        }

        public static void DeleteHotelFacilities(string Acco_id)
        {
            object result = null;
            MDMSVC.DC_Accommodation_Facility RQParams = new MDMSVC.DC_Accommodation_Facility();
            RQParams.Accommodation_Id = Guid.Parse(Acco_id);
            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteFacilitiesURI"], RQParams, typeof(DC_Accommodation_Facility), typeof(DC_Message), out result);
        }
        #endregion

        #region Contact
        public static void AddHotelContactsDetails(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            object result = null;

            MDMSVC.DC_Accommodation_Contact AC = new MDMSVC.DC_Accommodation_Contact();
            {
                foreach (var item in dt.data.accomodationData.accomodationInfo.contactDetails)
                {
                    AC.Accommodation_Contact_Id = Guid.NewGuid();
                    AC.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                    AC.Telephone = item.phone.countryCode + "-" + item.phone.cityCode + "-" + item.phone.number;
                    AC.Fax = "";// item.fax.countryCode + "-" + item.fax.cityCode + "-" + item.fax.number;
                    AC.Accommodation_Id = Guid.Parse(Acco_id);
                    AC.Create_Date = DateTime.Now;
                    AC.Create_User = "Kafka";
                    AC.Edit_Date = DateTime.Now;
                    AC.Edit_User = "Kafka";
                    AC.Email = item.emailAddress;
                    AC.WebSiteURL = item.website;
                    AC.IsActive = true;

                    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddContactsURI"], AC, typeof(DC_Accommodation_Contact), typeof(bool), out result);
                }
            };
        }

        public static void AddHotelContactsDetailsList(string Acco_id, MDMSVC.DC_Accomodation HotelData, RootObject dt)
        {
            object result = null;
            object res = null;
            string AppSetting = string.Empty;
            dynamic Obj = new ExpandoObject();
            dynamic TypeOf = new ExpandoObject();


            if (dt.data.accomodationData.accomodationInfo.contactDetails.Count > 0)
            {
                //Get country ,city Id
                MDMSVC.DC_City_Search_RQ RQ = new MDMSVC.DC_City_Search_RQ();
                RQ.Country_Name = dt.data.accomodationData.accomodationInfo.address.country;
                RQ.City_Name = dt.data.accomodationData.accomodationInfo.address.city;
                RQ.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
                RQ.Status = "ACTIVE";
                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Masters_CountryCityGet"], RQ, typeof(MDMSVC.DC_City_Search_RQ), typeof(List<MDMSVC.DC_City>), out res);
                List<DC_City> country_city_Id = (List<DC_City>)res;

                List<MDMSVC.DC_Accommodation_Contact> Lst = new List<MDMSVC.DC_Accommodation_Contact>();

                foreach (var item in dt.data.accomodationData.accomodationInfo.contactDetails)
                {
                    MDMSVC.DC_Accommodation_Contact AC = new MDMSVC.DC_Accommodation_Contact();

                    AC.Accommodation_Contact_Id = Guid.NewGuid();
                    AC.Accommodation_Id = Guid.Parse(Acco_id);
                    AC.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                    AC.Telephone = item.phone.countryCode + "-" + item.phone.cityCode + "-" + item.phone.number;
                    AC.Fax = "";// item.fax.countryCode + "-" + item.fax.cityCode + "-" + item.fax.number;
                    AC.WebSiteURL = item.website;
                    AC.Email = item.emailAddress;
                    AC.Create_Date = DateTime.Now;
                    AC.Create_User = "Kafka";
                    AC.Edit_Date = DateTime.Now;
                    AC.Edit_User = "Kafka";
                    AC.IsActive = true;
                    AC.Country_Id = (country_city_Id.Count > 0 ? country_city_Id[0].Country_Id : (Guid?)null);
                    AC.City_Id = (country_city_Id.Count > 0 ? (country_city_Id[0].City_Id) : (Guid?)null);

                    if (dt.data.accomodationData.accomodationInfo.contactDetails.Count > 1) { Lst.Add(AC); }
                    else { AppSetting = "Accomodation_AddContactsURI"; Obj = AC; TypeOf = new MDMSVC.DC_Accommodation_Contact(); }

                }

                if (dt.data.accomodationData.accomodationInfo.contactDetails.Count > 1) { Obj = Lst; AppSetting = "Accomodation_AddLstContactsURI"; TypeOf = new List<MDMSVC.DC_Accommodation_Contact>(); }

                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings[AppSetting], Obj, TypeOf.GetType(), typeof(bool), out result);
            }
        }

        public static void DeleteHotelContact(string Acco_id)
        {
            object result = null;
            MDMSVC.DC_Accommodation_Contact RQParams = new MDMSVC.DC_Accommodation_Contact();
            RQParams.Accommodation_Id = Guid.Parse(Acco_id);
            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteContactURI"], RQParams, typeof(DC_Accommodation_Contact), typeof(DC_Message), out result);
        }

        #endregion contact

        #region Status

        public static void AddAccomodationStatus(string Acco_id, MDMSVC.DC_Accomodation Status, RootObject dt)
        {
            try
            {
                object resultdeact = null;
                object result = null;
                MDMSVC.DC_Accommodation_Status AC = new MDMSVC.DC_Accommodation_Status();
                {
                    foreach (var item in dt.data.accomodationData.productStatus.deactivated)
                    {
                        if (item.@from != null)
                        {
                            string from = (item.@from).day + "-" + (item.@from).month + "-" + (item.@from).year;
                            AC.From = DateTime.Parse(from.ToString());
                        }
                        if (item.@from != null)
                        {
                            string to = (item.to).day + "-" + (item.to).month + "-" + (item.to).year;
                            AC.To = DateTime.Parse(to.ToString());
                        }
                        AC.Accommodation_Status_Id = Guid.NewGuid();
                        AC.Accommodation_Id = Guid.Parse(Acco_id);
                        AC.CompanyMarket = "";//item.marketName;
                        AC.DeactivationReason = item.reason;
                        AC.Status = "ACTIVE";
                        AC.IsActive = true;
                        AC.Create_Date = DateTime.Now;
                        AC.Create_User = "Kafka";
                        AC.Edit_Date = DateTime.Now;
                        AC.Edit_User = "Kafka";
                        Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddStatusURI"], AC, typeof(DC_Accommodation_Status), typeof(bool), out resultdeact);
                    }

                    if (dt.data.accomodationData.productStatus.@from != null)
                    {
                        string from = (dt.data.accomodationData.productStatus.@from).day + "-" + (dt.data.accomodationData.productStatus.@from).month + "-" + (dt.data.accomodationData.productStatus.@from).year;//dt.data.accomodationData.productStatus.from.ToString();// item.@from.day + "-" + item.from.month + "-" + item.from.year;
                        AC.From = DateTime.Parse(from.ToString());
                    }
                    if (dt.data.accomodationData.productStatus.@from != null)
                    {
                        string to = (dt.data.accomodationData.productStatus.to).day + "-" + (dt.data.accomodationData.productStatus.to).month + "-" + (dt.data.accomodationData.productStatus.to).year; //dt.data.accomodationData.productStatus.to.ToString(); //item.to.day + "-" + item.to.month + "-" + item.to.year;
                        AC.To = DateTime.Parse(to.ToString());
                    }
                    AC.Accommodation_Status_Id = Guid.NewGuid();
                    AC.Accommodation_Id = Guid.Parse(Acco_id);
                    AC.CompanyMarket = "";//item.marketName;
                    AC.DeactivationReason = dt.data.accomodationData.productStatus.reason;
                    AC.Status = AC.Status;
                    AC.IsActive = true;
                    AC.Create_Date = DateTime.Now;
                    AC.Create_User = "Kafka";
                    AC.Edit_Date = DateTime.Now;
                    AC.Edit_User = "Kafka";
                    //}
                    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddStatusURI"], AC, typeof(DC_Accommodation_Status), typeof(bool), out result);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public static void AddAccomodationStatusList(string Acco_id, MDMSVC.DC_Accomodation Status, RootObject dt)
        {
            try
            {
                string AppSetting = string.Empty;
                object resultdeact = null;
                object result = null;

                dynamic Obj = new ExpandoObject();

                if (dt.data.accomodationData.productStatus.deactivated.Count > 0)
                {
                    List<MDMSVC.DC_Accommodation_Status> Lst = new List<MDMSVC.DC_Accommodation_Status>();
                    foreach (var item in dt.data.accomodationData.productStatus.deactivated)
                    {
                        MDMSVC.DC_Accommodation_Status AC = new MDMSVC.DC_Accommodation_Status();
                        {
                            AC.Accommodation_Status_Id = Guid.NewGuid();
                            AC.Accommodation_Id = Guid.Parse(Acco_id);
                            AC.From = (item.@from != null ? DateTime.Parse((item.@from).year + "-" + (item.@from).month + "-" + item.@from.day) :
                                (dt.data.accomodationData.productStatus.@from != null ?
                                DateTime.Parse((dt.data.accomodationData.productStatus.@from).year + "-" +
                                (dt.data.accomodationData.productStatus.@from).month + "-" + dt.data.accomodationData.productStatus.@from.day) :
                                (DateTime?)null));

                            AC.To = (item.to != null ? DateTime.Parse((item.to).year + "-" + (item.to).month + "-" + item.to.day) :
                               (dt.data.accomodationData.productStatus.@to != null ?
                               DateTime.Parse((dt.data.accomodationData.productStatus.@to).year + "-" +
                               (dt.data.accomodationData.productStatus.@to).month + "-" + dt.data.accomodationData.productStatus.@to.day) :
                               (DateTime?)null));

                            AC.DeactivationReason = (item.@from != null ? item.reason : null);
                            AC.Status = dt.data.accomodationData.productStatus.status;
                            AC.CompanyMarket = (item.@from != null ? item.marketName : null);
                            AC.IsActive = false;
                            AC.Create_Date = DateTime.Now;
                            AC.Create_User = "Kafka";
                            AC.Edit_Date = DateTime.Now;
                            AC.Edit_User = "Kafka";

                            if (dt.data.accomodationData.productStatus.deactivated.Count > 1) { Lst.Add(AC); }
                            else { AppSetting = "Accomodation_AddStatusURI"; Obj = AC; }
                        }
                    }

                    if (dt.data.accomodationData.productStatus.deactivated.Count > 1) { Obj = Lst; AppSetting = "Accomodation_AddLstStatusURI"; }

                }
                else
                {
                    AppSetting = "Accomodation_AddStatusURI";

                    MDMSVC.DC_Accommodation_Status AC = new MDMSVC.DC_Accommodation_Status();
                    {
                        AC.Accommodation_Status_Id = Guid.NewGuid();
                        AC.Accommodation_Id = Guid.Parse(Acco_id);
                        AC.From = (dt.data.accomodationData.productStatus.@from != null ?
                            DateTime.Parse((dt.data.accomodationData.productStatus.@from).year + "-" +
                            (dt.data.accomodationData.productStatus.@from).month + "-" + dt.data.accomodationData.productStatus.@from.day) :
                            (DateTime?)null);

                        AC.To = (dt.data.accomodationData.productStatus.@to != null ?
                           DateTime.Parse((dt.data.accomodationData.productStatus.@to).year + "-" +
                           (dt.data.accomodationData.productStatus.@to).month + "-" + dt.data.accomodationData.productStatus.@to.day) :
                           (DateTime?)null);

                        AC.DeactivationReason = null;
                        AC.Status = dt.data.accomodationData.productStatus.status;
                        AC.CompanyMarket = null;
                        AC.IsActive = true;
                        AC.Create_Date = DateTime.Now;
                        AC.Create_User = "Kafka";
                        AC.Edit_Date = DateTime.Now;
                        AC.Edit_User = "Kafka";
                        Obj = AC;
                    }
                }

                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings[AppSetting], Obj, typeof(DC_Accommodation_Status), typeof(bool), out resultdeact);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void DeleteHotelStatus(string Acco_id)
        {
            object result = null;
            MDMSVC.DC_Accommodation_Status RQParams = new MDMSVC.DC_Accommodation_Status();
            RQParams.Accommodation_Id = Guid.Parse(Acco_id);
            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_DeleteStatusURI"], RQParams, typeof(DC_Accommodation_Status), typeof(DC_Message), out result);
        }
        #endregion status

        #region Room

        public static void GetRoomInfo(string Acco_id, RootObject dt)
        {
            MDMSVC.DC_Accommodation_RoomInfo RoomData = new MDMSVC.DC_Accommodation_RoomInfo();
            foreach (var item in dt.data.accomodationRoomData)
            {
                object result = null;
                Proxy.GetData(ProxyFor.MDM_CONSUMER, string.Format(System.Configuration.ConfigurationManager.AppSettings["Accomodation_RoomSearch"], Acco_id, item.roomId), typeof(List<DC_Accommodation_RoomInfo>), out result);
                List<DC_Accommodation_RoomInfo> search = (List<DC_Accommodation_RoomInfo>)result;

                if (search.Count > 0)
                {
                    RoomData.Accommodation_Id = new Guid(Acco_id);
                    RoomData.Accommodation_RoomInfo_Id = search[0].Accommodation_RoomInfo_Id;
                    RoomData.Legacy_Htl_Id = Convert.ToInt32(search[0].Legacy_Htl_Id);
                    RoomData.BathRoomType = null;
                    RoomData.BedType = item.bedType;
                    RoomData.Category = item.category;
                    RoomData.CompanyRoomCategory = null;
                    RoomData.Edit_Date = DateTime.Now;
                    RoomData.Edit_User = "Kafka";
                    //RoomData.Create_Date = DateTime.Now;
                    //RoomData.Create_User = "Kafka";
                    RoomData.FloorName = null;
                    RoomData.FloorNumber = null;
                    RoomData.RoomCategory = item.category;
                    RoomData.RoomDecor = null;
                    RoomData.RoomName = item.name;
                    RoomData.RoomSize = item.roomSize.ToString();
                    RoomData.RoomView = null;
                    RoomData.Smoking = null;
                    RoomData.Description = item.roomDescription;
                    RoomData.IsActive = true;
                    RoomData.RoomId = item.roomId;
                    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateRoomURI"], RoomData, typeof(DC_Accommodation_RoomInfo), typeof(bool), out result);
                }
                else
                {
                    RoomData.Accommodation_Id = new Guid(Acco_id);
                    RoomData.Accommodation_RoomInfo_Id = Guid.NewGuid();
                    RoomData.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);//Convert.ToInt32(searchAcco[0].CompanyHotelId);
                    RoomData.BathRoomType = null;
                    RoomData.BedType = item.bedType;
                    RoomData.Category = item.category;
                    RoomData.CompanyRoomCategory = null;
                    RoomData.Edit_Date = DateTime.Now;
                    RoomData.Edit_User = "Kafka";
                    RoomData.Create_Date = DateTime.Now;
                    RoomData.Create_User = "Kafka";
                    RoomData.FloorName = null;
                    RoomData.FloorNumber = null;
                    RoomData.RoomCategory = item.category;
                    RoomData.RoomDecor = null;
                    RoomData.RoomName = item.name;
                    RoomData.RoomSize = item.roomSize.ToString();
                    RoomData.RoomView = null;
                    RoomData.Smoking = null;
                    RoomData.Description = item.roomDescription;
                    RoomData.IsActive = true;
                    RoomData.RoomId = item.roomId;
                    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddRoomURI"], RoomData, typeof(DC_Accommodation_RoomInfo), typeof(bool), out result);
                }
            }
        }

        public static MDMSVC.DC_Accommodation_RoomInfo DataBindRoomInfo(MDMSVC.DC_Accommodation_RoomInfo ExistingRecordForUpdate, AccomodationRoomData item)
        {
            //MDMSVC.DC_Accommodation_RoomInfo RoomDataForUpdate = new MDMSVC.DC_Accommodation_RoomInfo();
            List<MDMSVC.DC_Accomodation_Roomamenities> _LstAmenities = new List<MDMSVC.DC_Accomodation_Roomamenities>();

            ExistingRecordForUpdate.Accommodation_Id = ExistingRecordForUpdate.Accommodation_Id;
            ExistingRecordForUpdate.Accommodation_RoomInfo_Id = ExistingRecordForUpdate.Accommodation_RoomInfo_Id;
            ExistingRecordForUpdate.Legacy_Htl_Id = ExistingRecordForUpdate.Legacy_Htl_Id;

            // Sample Turnary Operator Meaning >> When search.Property is not Null AND (RoomDataForUpdate is null or Not matching with Search.Property) 
            //Then Replace Else Keep Old Value.
            ExistingRecordForUpdate.RoomId = item.roomId;
            ExistingRecordForUpdate.RoomView = ExistingRecordForUpdate.RoomView;
            ExistingRecordForUpdate.RoomName = item.name;

            ExistingRecordForUpdate.Description = item.roomDescription;
            ExistingRecordForUpdate.RoomSize = Convert.ToString(item.roomSize);
            ExistingRecordForUpdate.RoomDecor = null;
            ExistingRecordForUpdate.Smoking = null;
            ExistingRecordForUpdate.FloorName = null;
            ExistingRecordForUpdate.FloorNumber = null;
            ExistingRecordForUpdate.BathRoomType = null;
            ExistingRecordForUpdate.BedType = item.bedType;
            ExistingRecordForUpdate.CompanyRoomCategory = null;
            ExistingRecordForUpdate.RoomCategory = item.category;
            ExistingRecordForUpdate.Category = item.category;
            ExistingRecordForUpdate.Create_User = ExistingRecordForUpdate.Create_User;
            ExistingRecordForUpdate.Create_Date = ExistingRecordForUpdate.Create_Date;
            ExistingRecordForUpdate.Edit_User = "Kafka";
            ExistingRecordForUpdate.Edit_Date = DateTime.Now;
            ExistingRecordForUpdate.IsActive = true;
            ExistingRecordForUpdate.TLGXAccoRoomId = ExistingRecordForUpdate.TLGXAccoRoomId;



            if (item.amenities.Count > 0)
            {
                ExistingRecordForUpdate.IsAmenityChanges = true;

                foreach (var itm in item.amenities)
                {
                    MDMSVC.DC_Accomodation_Roomamenities _Amenities = new MDMSVC.DC_Accomodation_Roomamenities();
                    _Amenities.RoomInfo_Id = ExistingRecordForUpdate.Accommodation_RoomInfo_Id;
                    _Amenities.Accommodation_Id = ExistingRecordForUpdate.Accommodation_Id;
                    _Amenities.isChargeable = itm.isChargeable;
                    _Amenities.name = itm.name;
                    _Amenities.type = itm.type;
                    _LstAmenities.Add(_Amenities);
                    _Amenities = null;
                }

                ExistingRecordForUpdate.Amenities = _LstAmenities.ToArray();
            }

            return ExistingRecordForUpdate;
        }

        //public static MDMSVC.DC_Accommodation_RoomInfo DataBindRoomInfo(MDMSVC.DC_Accommodation_RoomInfo ExistingRecordForUpdate, AccomodationRoomData item)
        //{
        //    //MDMSVC.DC_Accommodation_RoomInfo RoomDataForUpdate = new MDMSVC.DC_Accommodation_RoomInfo();
        //    List<MDMSVC.DC_Accomodation_Roomamenities> _LstAmenities = new List<MDMSVC.DC_Accomodation_Roomamenities>();

        //    ExistingRecordForUpdate.Accommodation_Id = search.Accommodation_Id;
        //    ExistingRecordForUpdate.Accommodation_RoomInfo_Id = search.Accommodation_RoomInfo_Id;
        //    ExistingRecordForUpdate.Legacy_Htl_Id = search.Legacy_Htl_Id;

        //    // Sample Turnary Operator Meaning >> When search.Property is not Null AND (RoomDataForUpdate is null or Not matching with Search.Property) 
        //    //Then Replace Else Keep Old Value.
        //    ExistingRecordForUpdate.RoomId = (string.IsNullOrEmpty(search.RoomId.Trim()) && (ExistingRecordForUpdate.RoomId == null ||
        //        ExistingRecordForUpdate.RoomId != search.RoomId) ? search.RoomId.Trim() : ExistingRecordForUpdate.RoomId);
        //    ExistingRecordForUpdate.RoomView = null;
        //    ExistingRecordForUpdate.RoomName = (string.IsNullOrEmpty(search.RoomName.Trim()) && (ExistingRecordForUpdate.RoomName == null ||
        //        ExistingRecordForUpdate.RoomName != search.RoomName) ? search.RoomId.Trim() : ExistingRecordForUpdate.RoomName);

        //    ExistingRecordForUpdate.Description = search.Description;
        //    ExistingRecordForUpdate.RoomSize = ExistingRecordForUpdate.RoomSize;
        //    ExistingRecordForUpdate.RoomDecor = null;
        //    ExistingRecordForUpdate.Smoking = null;
        //    ExistingRecordForUpdate.FloorName = null;
        //    ExistingRecordForUpdate.FloorNumber = null;
        //    ExistingRecordForUpdate.BathRoomType = null;
        //    ExistingRecordForUpdate.BedType = (string.IsNullOrEmpty(search.BedType.Trim()) && (ExistingRecordForUpdate.BedType == null ||
        //        ExistingRecordForUpdate.BedType != search.BedType) ? search.BedType.Trim() : ExistingRecordForUpdate.BedType);
        //    ExistingRecordForUpdate.CompanyRoomCategory = null;
        //    ExistingRecordForUpdate.RoomCategory = (string.IsNullOrEmpty(search.RoomCategory.Trim()) && (ExistingRecordForUpdate.RoomCategory == null ||
        //        ExistingRecordForUpdate.RoomCategory != search.RoomCategory) ? search.RoomCategory.Trim() : ExistingRecordForUpdate.RoomCategory);
        //    ExistingRecordForUpdate.Category = (string.IsNullOrEmpty(search.Category.Trim()) && (ExistingRecordForUpdate.Category == null ||
        //        ExistingRecordForUpdate.Category != search.Category) ? search.Category.Trim() : ExistingRecordForUpdate.Category);
        //    ExistingRecordForUpdate.Create_User = ExistingRecordForUpdate.Create_User;
        //    ExistingRecordForUpdate.Create_Date = ExistingRecordForUpdate.Create_Date;
        //    ExistingRecordForUpdate.Edit_User = "Kafka";
        //    ExistingRecordForUpdate.Edit_Date = DateTime.Now;

        //    // This pending for isDelete
        //    ExistingRecordForUpdate.IsActive = search.IsActive;
        //    ExistingRecordForUpdate.TLGXAccoRoomId = (ExistingRecordForUpdate.TLGXAccoRoomId != null ? ExistingRecordForUpdate.TLGXAccoRoomId : search.TLGXAccoRoomId);



        //    if (amenities.Count > 0)
        //    {
        //        ExistingRecordForUpdate.IsAmenityChanges = true;

        //        foreach (var itm in amenities)
        //        {
        //            MDMSVC.DC_Accomodation_Roomamenities _Amenities = new MDMSVC.DC_Accomodation_Roomamenities();
        //            _Amenities.RoomInfo_Id = ExistingRecordForUpdate.Accommodation_RoomInfo_Id;
        //            _Amenities.Accommodation_Id = ExistingRecordForUpdate.Accommodation_Id;
        //            _Amenities.isChargeable = itm.isChargeable;
        //            _Amenities.name = itm.name;
        //            _Amenities.type = itm.type;
        //            _LstAmenities.Add(_Amenities);
        //            _Amenities = null;
        //        }

        //        ExistingRecordForUpdate.Amenities = _LstAmenities;
        //    }

        //    return ExistingRecordForUpdate;
        //}

        public static void GetRoomInfoByAccoID(string Acco_id, RootObject dt)
        {

            dynamic objForAddProxy = new ExpandoObject();
            dynamic objForUpdateProxy = new ExpandoObject();

            List<MDMSVC.DC_Accommodation_RoomInfo> LstAddRoomData = new List<MDMSVC.DC_Accommodation_RoomInfo>();
            List<MDMSVC.DC_Accommodation_RoomInfo> LstUpdateRoomData = new List<MDMSVC.DC_Accommodation_RoomInfo>();

            object result = null;
            Proxy.GetData(ProxyFor.MDM_CONSUMER, string.Format(System.Configuration.ConfigurationManager.AppSettings["Accomodation_RoomSearchByAccoID"], Acco_id), typeof(List<DC_Accommodation_RoomInfo>), out result);

            IList<DC_Accommodation_RoomInfo> FindRoom = (IList<DC_Accommodation_RoomInfo>)result;
            string s = string.Empty;
            int UpdateFlag = 0;


            foreach (var item in dt.data.accomodationRoomData)
            {
                MDMSVC.DC_Accommodation_RoomInfo RoomData = new MDMSVC.DC_Accommodation_RoomInfo();
                MDMSVC.DC_Accommodation_RoomInfo RoomDataForUpdate = new MDMSVC.DC_Accommodation_RoomInfo();

                List<MDMSVC.DC_Accomodation_Roomamenities> _LstAmenities = new List<MDMSVC.DC_Accomodation_Roomamenities>();

                int cnt = FindRoom.Cast<DC_Accommodation_RoomInfo>().Where(x => x.TLGXAccoRoomId == item._id).Count();


                // External Array Unique Case Filter
                if (cnt > 0)
                {
                    if (UpdateFlag <= 0) { UpdateFlag = 1; }

                    MDMSVC.DC_Accommodation_RoomInfo ExistingRecordForUpdate = FindRoom.Cast<DC_Accommodation_RoomInfo>().Where(x => x.TLGXAccoRoomId == item._id).FirstOrDefault();


                    // List Initialization
                    if (dt.data.accomodationRoomData.Count > 1)
                    {
                        if (LstUpdateRoomData.Where(x => x.TLGXAccoRoomId == ExistingRecordForUpdate.TLGXAccoRoomId).Count() <= 0)
                        {
                            LstUpdateRoomData.Add(DataBindRoomInfo(ExistingRecordForUpdate, item));
                        }
                    }
                    else { objForUpdateProxy = DataBindRoomInfo(ExistingRecordForUpdate, item); }

                }
                else
                {
                    if (dt.data.accomodationRoomData.Count > 1)
                    {
                        // Internal Array Searching Unique Case Filter
                        if (LstAddRoomData.Where(x => x.TLGXAccoRoomId == item._id).Count() <= 0)
                        {
                            RoomData.Accommodation_Id = new Guid(Acco_id);
                            RoomData.Accommodation_RoomInfo_Id = Guid.NewGuid();
                            RoomData.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                            RoomData.RoomId = item.roomId;
                            RoomData.RoomView = null;
                            RoomData.RoomName = item.name;
                            RoomData.Description = item.roomDescription;
                            RoomData.RoomSize = Convert.ToString(item.roomSize);
                            RoomData.RoomDecor = null;
                            RoomData.Smoking = null;
                            RoomData.FloorName = null;
                            RoomData.FloorNumber = null;
                            RoomData.BathRoomType = null;
                            RoomData.BedType = item.bedType;
                            RoomData.CompanyRoomCategory = null;
                            RoomData.RoomCategory = item.category;
                            RoomData.Category = item.category;
                            RoomData.Create_User = "Kafka";
                            RoomData.Create_Date = DateTime.Now;
                            RoomData.Edit_User = "Kafka";
                            RoomData.Edit_Date = DateTime.Now;
                            RoomData.IsActive = true;
                            RoomData.TLGXAccoRoomId = item._id;

                            if (item.amenities.Count > 0)
                            {
                                RoomData.IsAmenityChanges = true;

                                foreach (var itm in item.amenities)
                                {
                                    MDMSVC.DC_Accomodation_Roomamenities _Amenities = new MDMSVC.DC_Accomodation_Roomamenities();
                                    _Amenities.RoomInfo_Id = RoomData.Accommodation_RoomInfo_Id;
                                    _Amenities.Accommodation_Id = new Guid(Acco_id);
                                    _Amenities.isChargeable = itm.isChargeable;
                                    _Amenities.name = itm.name;
                                    _Amenities.type = itm.type;
                                    _LstAmenities.Add(_Amenities);
                                    _Amenities = null;
                                }

                                RoomData.Amenities = _LstAmenities.ToArray();
                            }

                            LstAddRoomData.Add(RoomData);
                        }
                    }
                    else
                    {
                        RoomData.Accommodation_Id = new Guid(Acco_id);
                        RoomData.Accommodation_RoomInfo_Id = Guid.NewGuid();
                        RoomData.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                        RoomData.RoomId = item.roomId;
                        RoomData.RoomView = null;
                        RoomData.RoomName = item.name;
                        RoomData.Description = item.roomDescription;
                        RoomData.RoomSize = Convert.ToString(item.roomSize);
                        RoomData.RoomDecor = null;
                        RoomData.Smoking = null;
                        RoomData.FloorName = null;
                        RoomData.FloorNumber = null;
                        RoomData.BathRoomType = null;
                        RoomData.BedType = item.bedType;
                        RoomData.CompanyRoomCategory = null;
                        RoomData.RoomCategory = item.category;
                        RoomData.Category = item.category;
                        RoomData.Create_User = "Kafka";
                        RoomData.Create_Date = DateTime.Now;
                        RoomData.Edit_User = "Kafka";
                        RoomData.Edit_Date = DateTime.Now;
                        RoomData.IsActive = true;
                        RoomData.TLGXAccoRoomId = item._id;

                        if (item.amenities.Count > 0)
                        {
                            RoomData.IsAmenityChanges = true;

                            foreach (var itm in item.amenities)
                            {
                                MDMSVC.DC_Accomodation_Roomamenities _Amenities = new MDMSVC.DC_Accomodation_Roomamenities();
                                _Amenities.RoomInfo_Id = RoomData.Accommodation_RoomInfo_Id;
                                _Amenities.Accommodation_Id = new Guid(Acco_id);
                                _Amenities.isChargeable = itm.isChargeable;
                                _Amenities.name = itm.name;
                                _Amenities.type = itm.type;
                                _LstAmenities.Add(_Amenities);
                                _Amenities = null;
                            }

                            RoomData.Amenities = _LstAmenities.ToArray();
                        }

                        objForAddProxy = RoomData;
                    }
                }

                RoomData = null;
            }

            if (dt.data.accomodationRoomData.Count > 1) { objForAddProxy = LstAddRoomData; }

            if (UpdateFlag > 0) { objForUpdateProxy = LstUpdateRoomData; }

            //Accomodation_AddLstRoomURI


            // Add Proxy
            if (LstAddRoomData.Count > 0)
            {
                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings[((LstAddRoomData.Count > 1 ? "Accomodation_AddLstRoomURI" : "Accomodation_AddRoomURI"))],
               (LstAddRoomData.Count > 1 ? objForAddProxy : objForAddProxy[0]), (LstAddRoomData.Count > 1 ? objForAddProxy : objForAddProxy[0]).GetType(), typeof(bool), out result);
            }

            // Update proxy
            if (LstUpdateRoomData.Count > 0)
            {
                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings[((LstUpdateRoomData.Count > 1 ? "Accomodation_UpdateLstRoomURI" : "Accomodation_UpdateRoomURI"))],
                objForUpdateProxy, objForUpdateProxy.GetType(), typeof(bool), out result);
            }



            //foreach (var item in dt.data.accomodationRoomData)
            //{
            //if (search.Count > 0)
            //{
            //    RoomData.Accommodation_Id = new Guid(Acco_id);
            //    RoomData.Accommodation_RoomInfo_Id = search[0].Accommodation_RoomInfo_Id;
            //    RoomData.Legacy_Htl_Id = Convert.ToInt32(search[0].Legacy_Htl_Id);
            //    RoomData.BathRoomType = null;
            //    RoomData.BedType = item.bedType;
            //    RoomData.Category = item.category;
            //    RoomData.CompanyRoomCategory = null;
            //    RoomData.Edit_Date = DateTime.Now;
            //    RoomData.Edit_User = "Kafka";
            //    //RoomData.Create_Date = DateTime.Now;
            //    //RoomData.Create_User = "Kafka";
            //    RoomData.FloorName = null;
            //    RoomData.FloorNumber = null;
            //    RoomData.RoomCategory = item.category;
            //    RoomData.RoomDecor = null;
            //    RoomData.RoomName = item.name;
            //    RoomData.RoomSize = item.roomSize.ToString();
            //    RoomData.RoomView = null;
            //    RoomData.Smoking = null;
            //    RoomData.Description = item.roomDescription;
            //    RoomData.IsActive = true;
            //    RoomData.RoomId = item.roomId;
            //    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateRoomURI"], RoomData, typeof(DC_Accommodation_RoomInfo), typeof(bool), out result);
            //}
            //else
            //{
            //    RoomData.Accommodation_Id = new Guid(Acco_id);
            //    RoomData.Accommodation_RoomInfo_Id = Guid.NewGuid();
            //    RoomData.Legacy_Htl_Id = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);//Convert.ToInt32(searchAcco[0].CompanyHotelId);
            //    RoomData.BathRoomType = null;
            //    RoomData.BedType = item.bedType;
            //    RoomData.Category = item.category;
            //    RoomData.CompanyRoomCategory = null;
            //    RoomData.Edit_Date = DateTime.Now;
            //    RoomData.Edit_User = "Kafka";
            //    RoomData.Create_Date = DateTime.Now;
            //    RoomData.Create_User = "Kafka";
            //    RoomData.FloorName = null;
            //    RoomData.FloorNumber = null;
            //    RoomData.RoomCategory = item.category;
            //    RoomData.RoomDecor = null;
            //    RoomData.RoomName = item.name;
            //    RoomData.RoomSize = item.roomSize.ToString();
            //    RoomData.RoomView = null;
            //    RoomData.Smoking = null;
            //    RoomData.Description = item.roomDescription;
            //    RoomData.IsActive = true;
            //    RoomData.RoomId = item.roomId;
            //    Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddRoomURI"], RoomData, typeof(DC_Accommodation_RoomInfo), typeof(bool), out result);
            //}
            //}
        }
        #endregion Room

        public static DC_Accomodation AddData(List<DC_Stg_Kafka> Kafka, List<DC_Accomodation_Search_RS> search, RootObject dt, List<DC_City> country_city_Id, string row_id)
        {
            MDMSVC.DC_Accomodation HotelData = new DC_Accomodation();
            DateTime dtRating;
            object result = null;

            HotelData.Accommodation_Id = Guid.NewGuid();
            HotelData.CompanyName = dt.data.accomodationData.accomodationInfo.companyName;
            HotelData.CompanyHotelID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.FinanceControlID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.HotelRating = dt.data.accomodationData.accomodationInfo.rating;
            HotelData.CompanyRating = dt.data.accomodationData.accomodationInfo.companyRating;
            HotelData.HotelName = dt.data.accomodationData.accomodationInfo.name;
            HotelData.DisplayName = dt.data.accomodationData.accomodationInfo.displayName;
            HotelData.ProductCategory = "Accommodation";
            HotelData.ProductCategorySubType = (string.IsNullOrEmpty(string.IsNullOrWhiteSpace(dt.data.accomodationData.accomodationInfo.productCatSubType.Trim()).ToString()) ? "Hotel" : dt.data.accomodationData.accomodationInfo.productCatSubType);

            HotelData.RatingDate = (Convert.ToInt32(dt.data.accomodationData.accomodationInfo.rating) > 0 ?
                (DateTime.TryParse(Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.year) + "-" +
                Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.month) + "-" +
                Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.day), out dtRating) ? DateTime.Today.Date : dtRating) : DateTime.Today.Date);


            HotelData.TotalFloors = Convert.ToString(dt.data.accomodationData.accomodationInfo.noOfFloors);
            HotelData.TotalRooms = Convert.ToString(dt.data.accomodationData.accomodationInfo.noOfRooms);
            HotelData.CheckInTime = dt.data.accomodationData.accomodationInfo.checkInTime;
            HotelData.CheckOutTime = dt.data.accomodationData.accomodationInfo.checkOutTime;
            HotelData.InternalRemarks = dt.data.accomodationData.accomodationInfo.general.internalRemarks;
            HotelData.CompanyRecommended = dt.data.accomodationData.overview.isCompanyRecommended;

            Parallel.ForEach(dt.data.accomodationData.accomodationInfo.recommendedFor, word =>
            {
                if (dt.data.accomodationData.accomodationInfo.recommendedFor.FirstOrDefault().Equals(word)) { HotelData.RecommendedFor = ""; }
                HotelData.RecommendedFor += (dt.data.accomodationData.accomodationInfo.recommendedFor.LastOrDefault().Equals(word) ? word : word + ",");
            });

            Parallel.ForEach(dt.data.accomodationData.overview.hashTag, word =>
            {
                if (dt.data.accomodationData.overview.hashTag.FirstOrDefault().Equals(word)) { HotelData.Hashtag = ""; }
                HotelData.Hashtag += (dt.data.accomodationData.overview.hashTag.LastOrDefault().Equals(word) ? word : word + ",");
            });

            HotelData.Affiliation = null;
            HotelData.Reason = null;
            HotelData.Remarks = null;
            HotelData.CarbonFootPrint = null;
            HotelData.YearBuilt = dt.data.accomodationData.accomodationInfo.general.yearBuilt;
            HotelData.AwardsReceived = dt.data.accomodationData.accomodationInfo.general.awardsReceived;
            HotelData.IsActive = (Convert.ToString(dt.data.accomodationData.productStatus.status).ToUpper() == "ACTIVE" ? true : false);
            HotelData.IsMysteryProduct = dt.data.accomodationData.accomodationInfo.isMysteryProduct;
            HotelData.Create_User = "Kafka";
            HotelData.Create_Date = DateTime.Now;
            HotelData.Edit_User = "Kafka";
            HotelData.Edit_Date = DateTime.Now;
            HotelData.StreetName = dt.data.accomodationData.accomodationInfo.address.street;
            HotelData.StreetNumber = null;
            HotelData.Street3 = null;
            HotelData.Street4 = null;
            HotelData.Street5 = null;
            HotelData.PostalCode = dt.data.accomodationData.accomodationInfo.address.postalCode;
            HotelData.Town = null;
            HotelData.Location = null;
            HotelData.Area = null;
            HotelData.City = dt.data.accomodationData.accomodationInfo.address.city;
            HotelData.Country = dt.data.accomodationData.accomodationInfo.address.country;
            HotelData.SuburbDowntown = null;
            HotelData.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
            HotelData.Latitude = Convert.ToString(dt.data.accomodationData.accomodationInfo.address.geometry.coordinates[0]);
            HotelData.Longitude = Convert.ToString(dt.data.accomodationData.accomodationInfo.address.geometry.coordinates[1]);
            HotelData.LEGACY_COUNTRY = null;
            HotelData.LEGACY_CITY = null;
            HotelData.Country_ISO = null;
            HotelData.City_ISO = null;
            HotelData.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
            HotelData.State_ISO = null;
            HotelData.LEGACY_STATE = null;
            HotelData.Legacy_HTL_ID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.Google_Place_Id = null;

            // Below Property has Formula and that defined in Conzumer Services.
            // Do not Modify / Correct below property
            HotelData.FullAddress = "";

            if (country_city_Id.Count > 0)
            {
                HotelData.Country_Id = country_city_Id[0].Country_Id;
                HotelData.City_Id = country_city_Id[0].City_Id;
            }

            HotelData.InsertFrom = false;

            // Below Property has Formula and that defined in Conzumer Services.
            // Do not Modify / Correct below property
            HotelData.Latitude_Tx = "";
            HotelData.Longitude_Tx = "";

            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddInfoURI"], HotelData, typeof(List<MDMSVC.DC_Accomodation>), typeof(bool), out result);
            //AddHotelDescriptionList(Convert.ToString(HotelData.Accommodation_Id), HotelData, dt);
            //AddAccomodationFacilitiesList(Convert.ToString(HotelData.Accommodation_Id), HotelData, dt);
            //AddAccomodationStatusList(HotelData.Accommodation_Id.ToString(), HotelData, dt);
            AddHotelContactsDetailsList(HotelData.Accommodation_Id.ToString(), HotelData, dt);

            return HotelData;

        }

        public static DC_Accomodation UpdateData(List<DC_Stg_Kafka> Kafka, List<DC_Accomodation_Search_RS> search, RootObject dt, List<DC_City> country_city_Id, string row_id)
        {
            MDMSVC.DC_Accomodation HotelData = new DC_Accomodation();
            DateTime dtRating;
            object result = null;

            HotelData.Accommodation_Id = new Guid(search[0].AccomodationId);
            HotelData.CompanyName = dt.data.accomodationData.accomodationInfo.companyName;
            HotelData.CompanyHotelID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.FinanceControlID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.HotelRating = dt.data.accomodationData.accomodationInfo.rating;
            HotelData.CompanyRating = dt.data.accomodationData.accomodationInfo.companyRating;
            HotelData.HotelName = dt.data.accomodationData.accomodationInfo.name;
            HotelData.DisplayName = dt.data.accomodationData.accomodationInfo.displayName;
            HotelData.ProductCategory = "Accommodation";
            HotelData.ProductCategorySubType = (string.IsNullOrEmpty(string.IsNullOrWhiteSpace(dt.data.accomodationData.accomodationInfo.productCatSubType.Trim()).ToString()) ? "Hotel" : dt.data.accomodationData.accomodationInfo.productCatSubType);

            HotelData.RatingDate = (Convert.ToInt32(dt.data.accomodationData.accomodationInfo.rating) > 0 ?
             (DateTime.TryParse(Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.year) + "-" +
             Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.month) + "-" +
             Convert.ToString(dt.data.accomodationData.accomodationInfo.ratingDatedOn.day), out dtRating) ? DateTime.Today.Date : dtRating) : DateTime.Today.Date);

            HotelData.TotalFloors = Convert.ToString(dt.data.accomodationData.accomodationInfo.noOfFloors);

            HotelData.TotalFloors = Convert.ToString(dt.data.accomodationData.accomodationInfo.noOfFloors);
            HotelData.TotalRooms = dt.data.accomodationData.accomodationInfo.noOfRooms.ToString();
            HotelData.CheckInTime = dt.data.accomodationData.accomodationInfo.checkInTime;
            HotelData.CheckOutTime = dt.data.accomodationData.accomodationInfo.checkOutTime;
            HotelData.InternalRemarks = dt.data.accomodationData.accomodationInfo.general.internalRemarks;
            HotelData.CompanyRecommended = dt.data.accomodationData.overview.isCompanyRecommended;

            Parallel.ForEach(dt.data.accomodationData.accomodationInfo.recommendedFor, word =>
            {
                if (dt.data.accomodationData.accomodationInfo.recommendedFor.FirstOrDefault().Equals(word)) { HotelData.RecommendedFor = ""; }
                HotelData.RecommendedFor += (dt.data.accomodationData.accomodationInfo.recommendedFor.LastOrDefault().Equals(word) ? word : word + ",");
            });

            Parallel.ForEach(dt.data.accomodationData.overview.hashTag, word =>
            {
                if (dt.data.accomodationData.overview.hashTag.FirstOrDefault().Equals(word)) { HotelData.Hashtag = ""; }
                HotelData.Hashtag += (dt.data.accomodationData.overview.hashTag.LastOrDefault().Equals(word) ? word : word + ",");
            });

            HotelData.Chain = dt.data.accomodationData.accomodationInfo.chain;
            HotelData.Brand = dt.data.accomodationData.accomodationInfo.brand;
            HotelData.Affiliation = null;
            HotelData.Reason = null;
            HotelData.Remarks = null;
            HotelData.CarbonFootPrint = null;
            HotelData.YearBuilt = dt.data.accomodationData.accomodationInfo.general.yearBuilt;
            HotelData.AwardsReceived = dt.data.accomodationData.accomodationInfo.general.awardsReceived;
            HotelData.IsActive = (Convert.ToString(dt.data.accomodationData.productStatus.status).ToUpper() == "ACTIVE" ? true : false);
            HotelData.IsMysteryProduct = dt.data.accomodationData.accomodationInfo.isMysteryProduct;
            HotelData.Create_User = "Kafka";
            HotelData.Create_Date = DateTime.Now;
            HotelData.Edit_User = "Kafka";
            HotelData.Edit_Date = DateTime.Now;
            HotelData.StreetName = dt.data.accomodationData.accomodationInfo.address.street;
            HotelData.StreetNumber = null;
            HotelData.Street3 = null;
            HotelData.Street4 = null;
            HotelData.Street5 = null;
            HotelData.PostalCode = dt.data.accomodationData.accomodationInfo.address.postalCode;

            HotelData.Town = null;
            HotelData.Location = null;
            HotelData.Area = null;
            HotelData.City = dt.data.accomodationData.accomodationInfo.address.city;
            HotelData.Country = dt.data.accomodationData.accomodationInfo.address.country;
            HotelData.SuburbDowntown = null;

            HotelData.Latitude = Convert.ToString(dt.data.accomodationData.accomodationInfo.address.geometry.coordinates[0]);

            HotelData.Longitude = Convert.ToString(dt.data.accomodationData.accomodationInfo.address.geometry.coordinates[1]);

            HotelData.LEGACY_COUNTRY = null;
            HotelData.LEGACY_CITY = null;
            HotelData.Country_ISO = null;
            HotelData.City_ISO = null;
            HotelData.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
            HotelData.State_ISO = null;
            HotelData.LEGACY_STATE = null;
            HotelData.Legacy_HTL_ID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
            HotelData.Google_Place_Id = null;

            HotelData.FullAddress = "";

            // Below Property has Formula and that defined in Conzumer Services.
            // Do not Modify / Correct below property

            if (country_city_Id.Count > 0)
            {
                HotelData.Country_Id = country_city_Id[0].Country_Id;
                HotelData.City_Id = country_city_Id[0].City_Id;
            }

            HotelData.InsertFrom = false;

            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateURI"], HotelData, typeof(MDMSVC.DC_Accomodation), typeof(bool), out result);

            string Acco_id = search[0].AccomodationId;

            //Delete existing records and add new
            //DeleteHoteldescription(Acco_id);
            //DeleteHotelFacilities(Acco_id);
            //DeleteHotelStatus(Acco_id);

            //AddHotelDescriptionList(Acco_id, HotelData, dt);
            //AddAccomodationStatusList(Acco_id, HotelData, dt);
            //AddAccomodationFacilitiesList(Acco_id, HotelData, dt);

            DeleteHotelContact(Acco_id);
            AddHotelContactsDetailsList(Acco_id, HotelData, dt);


            return HotelData;
        }

        public static void GetPoll_Data1(object sender, System.Timers.ElapsedEventArgs e)
        {
            AdvancedConsumer svc = new AdvancedConsumer();
            object returnObject = null;
            List<Guid> Row = new List<Guid>();

            // Fetch All records from Table >> Stg_Kafka Where Status != "Read"
            Proxy.GetData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["GetPoll_Data"], typeof(List<DC_Stg_Kafka>), out returnObject);
            List<DC_Stg_Kafka> PollData = (List<DC_Stg_Kafka>)returnObject;

            List<DC_Accommodation_RoomInfo> lstDeleteInfo = new List<DC_Accommodation_RoomInfo>();

            foreach (var a in PollData)
            {
                bool isError = false;  //flag would remain false if no exception occurs 

                try
                {
                    string row_id = a.Row_Id.ToString();
                    string payload = a.PayLoad;
                    string topic = a.Topic;



                    if (topic.EndsWith(".PRODUCTACCO.PUB"))
                    {
                        object result = null;
                        object objresult = null;


                        var data = (JObject)JsonConvert.DeserializeObject(payload);

                        //{"method":"DELETE","data":"ACCOROOM4500046"}
                        if (Convert.ToString(data["method"]).ToUpper() == "DELETE")
                        {
                            lstDeleteInfo.AddRange(deleteRoomInfo(Convert.ToString(data["data"]).ToUpper()));

                            Proxy.GetData(ProxyFor.MDM_CONSUMER, (string.Format(System.Configuration.ConfigurationManager.AppSettings["Kafka_Select"], a.Row_Id)), typeof(List<DC_Stg_Kafka>), out result);
                            List<DC_Stg_Kafka> Kafka = (List<DC_Stg_Kafka>)result;

                            dynamic Dynamicz = new ExpandoObject();
                            if (lstDeleteInfo.Count > 1) { Dynamicz = lstDeleteInfo; } else { Dynamicz = lstDeleteInfo[0]; }

                            // Update proxy
                            if (lstDeleteInfo.Count > 0)
                            {
                                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings[((lstDeleteInfo.Count > 1 ? "Accomodation_UpdateLstRoomURI" : "Accomodation_UpdateRoomURI"))],
                                Dynamicz, Dynamicz.GetType(), typeof(bool), out result);
                            }

                            UpdateStg_KafkaInfo(Kafka, row_id);
                        }
                        else
                        {
                            RootObject dt = JsonConvert.DeserializeObject<RootObject>(payload);

                            if (dt.data.accomodationData.accomodationInfo != null)
                            {
                                string CommonProductId = dt.data.accomodationData.accomodationInfo.commonProductId;
                                MDMSVC.DC_Accomodation HotelData = new MDMSVC.DC_Accomodation();
                                MDMSVC.DC_Accomodation_Search_RQ RQParams = new MDMSVC.DC_Accomodation_Search_RQ();

                                //get kafka record for specific row_id
                                Proxy.GetData(ProxyFor.MDM_CONSUMER, (string.Format(System.Configuration.ConfigurationManager.AppSettings["Kafka_Select"], a.Row_Id)), typeof(List<DC_Stg_Kafka>), out result);
                                List<DC_Stg_Kafka> Kafka = (List<DC_Stg_Kafka>)result;

                                //search hotel using CompanyHotelId if record exist in accomodation table
                                RQParams.CompanyHotelId = Convert.ToInt32(CommonProductId);
                                RQParams.PageNo = 0;
                                RQParams.PageSize = int.MaxValue;
                                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_SearchURI"], RQParams, typeof(MDMSVC.DC_Accomodation_Search_RQ), typeof(List<MDMSVC.DC_Accomodation_Search_RS>), out objresult);
                                List<DC_Accomodation_Search_RS> search = (List<DC_Accomodation_Search_RS>)objresult;

                                object res = null;
                                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Masters_CountryCityGet"], objCity(dt), typeof(MDMSVC.DC_City_Search_RQ), typeof(List<MDMSVC.DC_City>), out res);
                                List<DC_City> country_city_Id = (List<DC_City>)res;

                                if (search.Count > 0)
                                {
                                    HotelData = UpdateData(Kafka, search, dt, country_city_Id, Convert.ToString(a.Row_Id));
                                }
                                else
                                {
                                    HotelData = AddData(Kafka, search, dt, country_city_Id, Convert.ToString(a.Row_Id));
                                }

                                //Manage Room Info
                                if (dt.data.accomodationRoomData.Count > 0)
                                {
                                    //GetRoomInfoByAccoID(Convert.ToString(HotelData.Accommodation_Id), dt);
                                    GetRoomInfoByAccoID(Convert.ToString(HotelData.Accommodation_Id), dt);
                                }

                                UpdateStg_KafkaInfo(Kafka, row_id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                }
                finally
                {
                    svc.Dispose();
                }
                if (isError) continue;
            }
        }

        public static DC_City_Search_RQ objCity(RootObject dt)
        {
            MDMSVC.DC_City_Search_RQ RQ = new MDMSVC.DC_City_Search_RQ();
            RQ.Country_Name = dt.data.accomodationData.accomodationInfo.address.country;
            RQ.City_Name = dt.data.accomodationData.accomodationInfo.address.city;
            RQ.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
            RQ.Status = "ACTIVE";
            return RQ;
        }

        public static List<DC_Accommodation_RoomInfo> deleteRoomInfo(string Acco_id)
        {
            object result = null;
            Proxy.GetData(ProxyFor.MDM_CONSUMER, string.Format(System.Configuration.ConfigurationManager.AppSettings["Accomodation_RoomSearchByAccoID"], "ACCOID-" + Acco_id), typeof(List<DC_Accommodation_RoomInfo>), out result);
            return (List<DC_Accommodation_RoomInfo>)result;
        }

        public static void GetPoll_Data(object sender, System.Timers.ElapsedEventArgs e)
        {
            object returnObject = null;
            //get all records from stg_kafka which are unread
            Proxy.GetData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["GetPoll_Data"], typeof(List<DC_Stg_Kafka>), out returnObject);
            List<DC_Stg_Kafka> PollData = (List<DC_Stg_Kafka>)returnObject;
            //try
            //{
            foreach (var a in PollData)
            {
                bool isError = false;  //flag would remain false if no exception occurs 
                try
                {
                    string row_id = a.Row_Id.ToString();
                    string payload = a.PayLoad;
                    string topic = a.Topic;
                    if (topic.EndsWith(".PRODUCTACCO.PUB"))
                    {
                        object result = null;
                        object objresult = null;
                        object resultGeo = null;
                        RootObject dt = JsonConvert.DeserializeObject<RootObject>(payload);

                        if (dt.data.accomodationData.accomodationInfo != null)
                        {
                            string CommonProductId = dt.data.accomodationData.accomodationInfo.commonProductId;
                            MDMSVC.DC_Accomodation HotelData = new MDMSVC.DC_Accomodation();
                            MDMSVC.DC_Accomodation_Search_RQ RQParams = new MDMSVC.DC_Accomodation_Search_RQ();

                            //get kafka record for specific row_id
                            Proxy.GetData(ProxyFor.MDM_CONSUMER, (string.Format(System.Configuration.ConfigurationManager.AppSettings["Kafka_Select"], a.Row_Id)), typeof(List<DC_Stg_Kafka>), out result);
                            List<DC_Stg_Kafka> Kafka = (List<DC_Stg_Kafka>)result;

                            //search hotel using CompanyHotelId if record exist in accomodation table
                            RQParams.CompanyHotelId = Convert.ToInt32(CommonProductId);
                            RQParams.PageNo = 0;
                            RQParams.PageSize = int.MaxValue;
                            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_SearchURI"], RQParams, typeof(MDMSVC.DC_Accomodation_Search_RQ), typeof(List<MDMSVC.DC_Accomodation_Search_RS>), out objresult);
                            List<DC_Accomodation_Search_RS> search = (List<DC_Accomodation_Search_RS>)objresult;

                            //Get country ,city Id
                            MDMSVC.DC_City_Search_RQ RQ = new MDMSVC.DC_City_Search_RQ();
                            RQ.Country_Name = dt.data.accomodationData.accomodationInfo.address.country;
                            RQ.City_Name = dt.data.accomodationData.accomodationInfo.address.city;
                            RQ.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
                            RQ.Status = "ACTIVE";

                            //Get Geo Location
                            MDMSVC.DC_Address_Physical AP = new MDMSVC.DC_Address_Physical();
                            AP.CityAreaOrDistrict = dt.data.accomodationData.accomodationInfo.address.city;
                            AP.CityOrTownOrVillage = dt.data.accomodationData.accomodationInfo.address.city;
                            AP.Country = dt.data.accomodationData.accomodationInfo.address.country;
                            AP.CountyOrState = dt.data.accomodationData.accomodationInfo.address.country;
                            AP.CountyOrState = dt.data.accomodationData.accomodationInfo.address.state;
                            AP.Street = dt.data.accomodationData.accomodationInfo.address.street;
                            AP.PostalCode = dt.data.accomodationData.accomodationInfo.address.postalCode;

                            // Proxy.PostData(ProxyFor.MDM_CONSUMER, ConfigurationManager.AppSettings["GeoLocation_ByAddress"], AP, typeof(DC_Address_Physical), typeof(DC_GeoLocation), out resultGeo);
                            var objListGeoAddress = resultGeo;

                            object res = null;
                            Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Masters_CountryCityGet"], RQ, typeof(MDMSVC.DC_City_Search_RQ), typeof(List<MDMSVC.DC_City>), out res);
                            List<DC_City> country_city_Id = (List<DC_City>)res;

                            if (search.Count > 0)
                            {
                                HotelData.Accommodation_Id = new Guid(search[0].AccomodationId);
                                HotelData.ProductCategory = "Accommodation";
                                HotelData.ProductCategorySubType = "Hotel";
                                HotelData.Affiliation = null;
                                HotelData.Area = "";
                                HotelData.Brand = "";
                                HotelData.Chain = "";
                                HotelData.Country = dt.data.accomodationData.accomodationInfo.address.country;
                                HotelData.City = dt.data.accomodationData.accomodationInfo.address.city;
                                HotelData.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
                                HotelData.CompanyRating = dt.data.accomodationData.accomodationInfo.companyRating;
                                HotelData.HotelRating = dt.data.accomodationData.accomodationInfo.rating;
                                HotelData.SuburbDowntown = null;
                                HotelData.Location = "";
                                HotelData.AwardsReceived = dt.data.accomodationData.accomodationInfo.general.awardsReceived;
                                HotelData.CheckInTime = dt.data.accomodationData.accomodationInfo.checkInTime;
                                HotelData.CheckOutTime = dt.data.accomodationData.accomodationInfo.checkOutTime;
                                HotelData.DisplayName = dt.data.accomodationData.accomodationInfo.displayName;
                                HotelData.HotelName = dt.data.accomodationData.accomodationInfo.name;
                                HotelData.InternalRemarks = dt.data.accomodationData.accomodationInfo.general.internalRemarks;
                                if (((DC_GeoLocation)resultGeo) != null)
                                {
                                    HotelData.Latitude = ((DC_GeoLocation)resultGeo).results[0].geometry.location.lat.ToString();
                                    HotelData.Longitude = ((DC_GeoLocation)resultGeo).results[0].geometry.location.lng.ToString();
                                }
                                HotelData.PostalCode = dt.data.accomodationData.accomodationInfo.address.postalCode;
                                HotelData.RatingDate = DateTime.Today.Date;
                                HotelData.StreetName = dt.data.accomodationData.accomodationInfo.address.street;
                                HotelData.StreetNumber = "";
                                HotelData.Street3 = "";
                                HotelData.Street4 = "";
                                HotelData.Street5 = "";
                                HotelData.TotalFloors = "";
                                HotelData.TotalRooms = dt.data.accomodationData.accomodationInfo.noOfRooms.ToString();
                                HotelData.YearBuilt = dt.data.accomodationData.accomodationInfo.general.yearBuilt;
                                HotelData.CompanyName = dt.data.accomodationData.accomodationInfo.companyName;
                                HotelData.CompanyRecommended = dt.data.accomodationData.overview.isCompanyRecommended;
                                HotelData.FinanceControlID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                                HotelData.CompanyHotelID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                                HotelData.Edit_Date = DateTime.Now;
                                HotelData.Edit_User = "Kafka";
                                HotelData.IsActive = true;
                                HotelData.IsMysteryProduct = false;
                                HotelData.IsActive = true;
                                HotelData.InsertFrom = false;

                                HotelData.Google_Place_Id = "";
                                HotelData.CheckInTime = "";
                                HotelData.CheckOutTime = "";

                                if (country_city_Id.Count > 0)
                                {
                                    HotelData.Country_Id = country_city_Id[0].Country_Id;

                                    HotelData.City_Id = country_city_Id[0].City_Id;
                                }

                                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_UpdateURI"], HotelData, typeof(MDMSVC.DC_Accomodation), typeof(bool), out result);

                                string Acco_id = search[0].AccomodationId;
                                //Delete existing records and add new
                                DeleteHoteldescription(Acco_id);
                                DeleteHotelFacilities(Acco_id);
                                DeleteHotelStatus(Acco_id);
                                DeleteHotelContact(Acco_id);
                                AddHotelDescription(Acco_id, HotelData, dt);
                                AddAccomodationFacilities(Acco_id, HotelData, dt);
                                AddAccomodationStatus(Acco_id, HotelData, dt);
                                AddHotelContactsDetails(Acco_id, HotelData, dt);
                                UpdateStg_KafkaInfo(Kafka, row_id);

                                if (dt.data.accomodationRoomData.Count > 0)
                                {
                                    GetRoomInfo(Acco_id, dt);
                                }
                            }
                            else
                            {
                                HotelData.Accommodation_Id = Guid.NewGuid();
                                HotelData.ProductCategory = "Accommodation";
                                HotelData.ProductCategorySubType = "Hotel";
                                HotelData.Affiliation = null;
                                HotelData.Area = "";
                                HotelData.Brand = "";
                                HotelData.Chain = "";
                                HotelData.Country = dt.data.accomodationData.accomodationInfo.address.country;
                                HotelData.City = dt.data.accomodationData.accomodationInfo.address.city;
                                HotelData.State_Name = dt.data.accomodationData.accomodationInfo.address.state;
                                HotelData.CompanyRating = dt.data.accomodationData.accomodationInfo.companyRating;
                                HotelData.HotelRating = dt.data.accomodationData.accomodationInfo.rating;
                                HotelData.SuburbDowntown = null;
                                HotelData.Location = "";
                                HotelData.AwardsReceived = dt.data.accomodationData.accomodationInfo.general.awardsReceived;
                                HotelData.CheckInTime = dt.data.accomodationData.accomodationInfo.checkInTime;
                                HotelData.CheckOutTime = dt.data.accomodationData.accomodationInfo.checkOutTime;
                                HotelData.DisplayName = dt.data.accomodationData.accomodationInfo.displayName;
                                HotelData.HotelName = dt.data.accomodationData.accomodationInfo.name;
                                HotelData.InternalRemarks = dt.data.accomodationData.accomodationInfo.general.internalRemarks;
                                if (((DC_GeoLocation)resultGeo) != null)
                                {
                                    HotelData.Latitude = ((DC_GeoLocation)resultGeo).results[0].geometry.location.lat.ToString();
                                    HotelData.Longitude = ((DC_GeoLocation)resultGeo).results[0].geometry.location.lng.ToString();
                                }
                                HotelData.PostalCode = dt.data.accomodationData.accomodationInfo.address.postalCode;
                                HotelData.RatingDate = DateTime.Today.Date;
                                HotelData.StreetName = dt.data.accomodationData.accomodationInfo.address.street;
                                HotelData.StreetNumber = "";
                                HotelData.Street3 = "";
                                HotelData.Street4 = "";
                                HotelData.Street5 = "";
                                HotelData.TotalFloors = "";
                                HotelData.TotalRooms = dt.data.accomodationData.accomodationInfo.noOfRooms.ToString();
                                HotelData.YearBuilt = dt.data.accomodationData.accomodationInfo.general.yearBuilt;
                                HotelData.CompanyName = dt.data.accomodationData.accomodationInfo.companyName;
                                HotelData.CompanyRecommended = dt.data.accomodationData.overview.isCompanyRecommended;
                                HotelData.FinanceControlID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                                HotelData.CompanyHotelID = Convert.ToInt32(dt.data.accomodationData.accomodationInfo.commonProductId);
                                HotelData.Edit_Date = DateTime.Now;
                                HotelData.Edit_User = "Kafka";
                                HotelData.IsActive = true;
                                HotelData.IsMysteryProduct = false;
                                HotelData.IsActive = true;
                                HotelData.InsertFrom = false;
                                HotelData.Create_Date = DateTime.Now;
                                HotelData.Create_User = "Kafka [" + dt.data.accomodationData.createdBy + "]";
                                HotelData.Google_Place_Id = "";
                                HotelData.CheckInTime = "";
                                HotelData.CheckOutTime = "";
                                if (country_city_Id.Count > 0)
                                {
                                    HotelData.Country_Id = country_city_Id[0].Country_Id;

                                    HotelData.City_Id = country_city_Id[0].City_Id;
                                }

                                Proxy.PostData(ProxyFor.MDM_CONSUMER, System.Configuration.ConfigurationManager.AppSettings["Accomodation_AddInfoURI"], HotelData, typeof(MDMSVC.DC_Accomodation), typeof(bool), out result);

                                //Delete existing records and add new
                                //DeleteHoteldescription(HotelData.Accommodation_Id.ToString());
                                //DeleteHotelFacilities(HotelData.Accommodation_Id.ToString());
                                //DeleteHotelStatus(HotelData.Accommodation_Id.ToString());
                                //DeleteHotelContact(HotelData.Accommodation_Id.ToString());
                                AddHotelDescription(HotelData.Accommodation_Id.ToString(), HotelData, dt);
                                AddAccomodationFacilities(HotelData.Accommodation_Id.ToString(), HotelData, dt);
                                AddAccomodationStatus(HotelData.Accommodation_Id.ToString(), HotelData, dt);
                                AddHotelContactsDetails(HotelData.Accommodation_Id.ToString(), HotelData, dt);
                                UpdateStg_KafkaInfo(Kafka, row_id);

                                //Room Info Add/update
                                if (dt.data.accomodationRoomData.Count > 0)
                                {
                                    GetRoomInfo(HotelData.Accommodation_Id.ToString(), dt);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                }
                if (isError) continue;
            }
        }
    }
}
