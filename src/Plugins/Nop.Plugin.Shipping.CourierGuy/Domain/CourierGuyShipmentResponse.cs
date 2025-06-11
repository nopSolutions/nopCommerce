using Newtonsoft.Json;
namespace Nop.Plugin.Shipping.CourierGuy.Domain;


// Root myDeserializedClass = JsonConvert.Deserializestring<Root>(myJsonResponse);
    public class Account
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("branch_id")]
        public int? BranchId { get; set; }

        [JsonProperty("branch_name")]
        public string BranchName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("account_code")]
        public string AccountCode { get; set; }

        [JsonProperty("tracking_prefix")]
        public string TrackingPrefix { get; set; }

        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        [JsonProperty("credit_limit")]
        public int? CreditLimit { get; set; }

        [JsonProperty("payment_terms")]
        public int? PaymentTerms { get; set; }

        [JsonProperty("invoice_frequency")]
        public string InvoiceFrequency { get; set; }

        [JsonProperty("invoice_frequency_time")]
        public int? InvoiceFrequencyTime { get; set; }

        [JsonProperty("balance")]
        public double? Balance { get; set; }

        [JsonProperty("credit_allocation_balance")]
        public int? CreditAllocationBalance { get; set; }

        [JsonProperty("is_on_hold")]
        public bool? IsOnHold { get; set; }

        [JsonProperty("disable_auto_hold")]
        public bool? DisableAutoHold { get; set; }

        [JsonProperty("temp_disable_auto_hold")]
        public bool? TempDisableAutoHold { get; set; }

        [JsonProperty("support_email")]
        public string SupportEmail { get; set; }

        [JsonProperty("charge_discrepancy_threshold_override")]
        public int? ChargeDiscrepancyThresholdOverride { get; set; }

        [JsonProperty("invoice_allocation_method")]
        public string InvoiceAllocationMethod { get; set; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("time_created")]
        public DateTime? TimeCreated { get; set; }

        [JsonProperty("time_modified")]
        public DateTime? TimeModified { get; set; }

        [JsonProperty("default_collection_address_id")]
        public int? DefaultCollectionAddressId { get; set; }

        [JsonProperty("custom_fields")]
        public string CustomFields { get; set; }

        [JsonProperty("force_show_billing_transactions")]
        public bool? ForceShowBillingTransactions { get; set; }

        [JsonProperty("invoice_trigger_shipment_status")]
        public string InvoiceTriggerShipmentStatus { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("trading_start_date")]
        public DateTime? TradingStartDate { get; set; }

        [JsonProperty("current_rate_group_id")]
        public int? CurrentRateGroupId { get; set; }

        [JsonProperty("current_rate_group_name")]
        public string CurrentRateGroupName { get; set; }

        [JsonProperty("pod_method")]
        public string PodMethod { get; set; }

        [JsonProperty("pod_method_visible_on_shipment")]
        public bool? PodMethodVisibleOnShipment { get; set; }

        [JsonProperty("include_unencrypted_pod_pin")]
        public bool? IncludeUnencryptedPodPin { get; set; }

        [JsonProperty("disable_out_for_delivery_sms")]
        public bool? DisableOutForDeliverySms { get; set; }

        [JsonProperty("disable_out_for_delivery_email")]
        public bool? DisableOutForDeliveryEmail { get; set; }

        [JsonProperty("disable_driver_app_notification_sms")]
        public bool? DisableDriverAppNotificationSms { get; set; }

        [JsonProperty("disable_driver_app_notification_email")]
        public bool? DisableDriverAppNotificationEmail { get; set; }

        [JsonProperty("disable_account_in_arrears_checks")]
        public bool? DisableAccountInArrearsChecks { get; set; }

        [JsonProperty("override_tracking_prefixes")]
        public bool? OverrideTrackingPrefixes { get; set; }
    }

    public partial class CollectionAddress
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("gs_hash")]
        public string GsHash { get; set; }

        [JsonProperty("time_modified")]
        public DateTime? TimeModified { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("entered_address")]
        public string EnteredAddress { get; set; }

        [JsonProperty("geo_local_area")]
        public string GeoLocalArea { get; set; }

        [JsonProperty("geo_city")]
        public string GeoCity { get; set; }

        [JsonProperty("time_created")]
        public DateTime? TimeCreated { get; set; }
    }

    public partial class CollectionContact
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("shipment_id")]
        public int? ShipmentId { get; set; }

    }

    public partial class DeliveryAddress
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("gs_hash")]
        public string GsHash { get; set; }

        [JsonProperty("time_modified")]
        public DateTime? TimeModified { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("entered_address")]
        public string EnteredAddress { get; set; }

        [JsonProperty("geo_local_area")]
        public string GeoLocalArea { get; set; }

        [JsonProperty("geo_city")]
        public string GeoCity { get; set; }

        [JsonProperty("time_created")]
        public DateTime? TimeCreated { get; set; }
    }

    public partial class DeliveryContact
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("shipment_id")]
        public int? ShipmentId { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("shipment_tracking_reference")]
        public string ShipmentTrackingReference { get; set; }

        [JsonProperty("service_level_code")]
        public string ServiceLevelCode { get; set; }

        [JsonProperty("parcels")]
        public List<CourierGuyRateRequest.Parcel> Parcels { get; set; }

        [JsonProperty("charged_weight")]
        public int? ChargedWeight { get; set; }

        [JsonProperty("estimated_weight")]
        public int? EstimatedWeight { get; set; }

        [JsonProperty("shipment_id")]
        public int? ShipmentId { get; set; }
    }

    public partial class Parcel
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tracking_reference")]
        public string TrackingReference { get; set; }

        [JsonProperty("packaging")]
        public string Packaging { get; set; }

        [JsonProperty("shipment_id")]
        public int? ShipmentId { get; set; }

        [JsonProperty("shipment")]
        public string Shipment { get; set; }

        [JsonProperty("actual_length_cm")]
        public int? ActualLengthCm { get; set; }

        [JsonProperty("actual_width_cm")]
        public int? ActualWidthCm { get; set; }

        [JsonProperty("actual_height_cm")]
        public int? ActualHeightCm { get; set; }

        [JsonProperty("actual_weight_kg")]
        public int? ActualWeightKg { get; set; }

        [JsonProperty("time_created")]
        public DateTime? TimeCreated { get; set; }
    }

    public class Provider
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("time_created")]
        public string TimeCreated { get; set; }

        [JsonProperty("invoice_counter")]
        public int? InvoiceCounter { get; set; }

        [JsonProperty("credit_note_counter")]
        public int? CreditNoteCounter { get; set; }

        [JsonProperty("interhub_transfer_counter")]
        public int? InterhubTransferCounter { get; set; }

        [JsonProperty("hik_vision_last_tracking_event_id")]
        public int? HikVisionLastTrackingEventId { get; set; }
    }

    public partial class Rate
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("shipment_id")]
        public int? ShipmentId { get; set; }

        [JsonProperty("rate_revision_id")]
        public int? RateRevisionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("type_id")]
        public int? TypeId { get; set; }

        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("vat")]
        public double? Vat { get; set; }

        [JsonProperty("vat_type")]
        public string VatType { get; set; }

        [JsonProperty("valid_from")]
        public DateTime? ValidFrom { get; set; }

        [JsonProperty("valid_until")]
        public DateTime? ValidUntil { get; set; }

        [JsonProperty("is_opt_in")]
        public bool? IsOptIn { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("billing_transaction_id")]
        public int? BillingTransactionId { get; set; }
    }

    public class CourierGuyShipmentResponse
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("provider_id")]
        public int? ProviderId { get; set; }

        [JsonProperty("provider")]
        public Provider Provider { get; set; }

        [JsonProperty("account_id")]
        public int? AccountId { get; set; }

        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("short_tracking_reference")]
        public string ShortTrackingReference { get; set; }

        [JsonProperty("customer_reference")]
        public string CustomerReference { get; set; }

        [JsonProperty("latest_tracking_event_time")]
        public DateTime? LatestTrackingEventTime { get; set; }

        [JsonProperty("time_created")]
        public DateTime? TimeCreated { get; set; }

        [JsonProperty("time_modified")]
        public DateTime? TimeModified { get; set; }

        [JsonProperty("modified_by")]
        public string ModifiedBy { get; set; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }

        [JsonProperty("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("collection_min_date")]
        public DateTime? CollectionMinDate { get; set; }

        [JsonProperty("collection_after")]
        public string CollectionAfter { get; set; }

        [JsonProperty("collection_before")]
        public string CollectionBefore { get; set; }

        [JsonProperty("delivery_min_date")]
        public DateTime? DeliveryMinDate { get; set; }

        [JsonProperty("delivery_after")]
        public string DeliveryAfter { get; set; }

        [JsonProperty("delivery_before")]
        public string DeliveryBefore { get; set; }

        [JsonProperty("collection_request_id")]
        public int? CollectionRequestId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("delivery_agent_id")]
        public string DeliveryAgentId { get; set; }

        [JsonProperty("collection_agent_id")]
        public string CollectionAgentId { get; set; }

        [JsonProperty("original_delivery_agent_id")]
        public string OriginalDeliveryAgentId { get; set; }

        [JsonProperty("original_collection_agent_id")]
        public string OriginalCollectionAgentId { get; set; }

        [JsonProperty("collection_agent_zone")]
        public string CollectionAgentZone { get; set; }

        [JsonProperty("collection_agent_routing_number")]
        public string CollectionAgentRoutingNumber { get; set; }

        [JsonProperty("delivery_agent_zone")]
        public string DeliveryAgentZone { get; set; }

        [JsonProperty("delivery_agent_routing_number")]
        public string DeliveryAgentRoutingNumber { get; set; }

        [JsonProperty("delivery_read_by_driver_time")]
        public string DeliveryReadByDriverTime { get; set; }

        [JsonProperty("collection_read_by_driver_time")]
        public string CollectionReadByDriverTime { get; set; }

        [JsonProperty("collection_address_id")]
        public int? CollectionAddressId { get; set; }

        [JsonProperty("collection_address")]
        public CollectionAddress CollectionAddress { get; set; }

        [JsonProperty("collection_address_book_id")]
        public int? CollectionAddressBookId { get; set; }

        [JsonProperty("collection_contact_id")]
        public int? CollectionContactId { get; set; }

        [JsonProperty("collection_contact")]
        public CollectionContact CollectionContact { get; set; }

        [JsonProperty("delivery_address_id")]
        public int? DeliveryAddressId { get; set; }

        [JsonProperty("delivery_address")]
        public DeliveryAddress DeliveryAddress { get; set; }

        [JsonProperty("delivery_address_book_id")]
        public int? DeliveryAddressBookId { get; set; }

        [JsonProperty("delivery_contact_id")]
        public int? DeliveryContactId { get; set; }

        [JsonProperty("delivery_contact")]
        public DeliveryContact DeliveryContact { get; set; }

        [JsonProperty("responsible_contact_id")]
        public int? ResponsibleContactId { get; set; }

        [JsonProperty("parcels")]
        public List<CourierGuyRateRequest.Parcel> Parcels { get; set; }

        [JsonProperty("service_level_code")]
        public string ServiceLevelCode { get; set; }

        [JsonProperty("service_level_name")]
        public string ServiceLevelName { get; set; }

        [JsonProperty("service_level_id")]
        public int? ServiceLevelId { get; set; }

        [JsonProperty("rate")]
        public decimal? Rate { get; set; }

        [JsonProperty("original_rate")]
        public decimal? OriginalRate { get; set; }

        [JsonProperty("previous_rate")]
        public decimal? PreviousRate { get; set; }

        [JsonProperty("charged_weight")]
        public decimal? ChargedWeight { get; set; }

        [JsonProperty("actual_weight")]
        public decimal? ActualWeight { get; set; }

        [JsonProperty("volumetric_weight")]
        public decimal? VolumetricWeight { get; set; }

        [JsonProperty("declared_value")]
        public decimal? DeclaredValue { get; set; }

        [JsonProperty("rates")]
        public List<Rate> Rates { get; set; }

        [JsonProperty("special_instructions_collection")]
        public string SpecialInstructionsCollection { get; set; }

        [JsonProperty("special_instructions_delivery")]
        public string SpecialInstructionsDelivery { get; set; }

        [JsonProperty("proof_of_delivery_pin_hashed")]
        public string ProofOfDeliveryPinHashed { get; set; }

        [JsonProperty("collected_date")]
        public string CollectedDate { get; set; }

        [JsonProperty("original_collected_date")]
        public string OriginalCollectedDate { get; set; }

        [JsonProperty("delivered_date")]
        public string DeliveredDate { get; set; }

        [JsonProperty("is_return")]
        public bool? IsReturn { get; set; }

        [JsonProperty("estimated_collection")]
        public DateTime? EstimatedCollection { get; set; }

        [JsonProperty("original_estimated_collection")]
        public DateTime? OriginalEstimatedCollection { get; set; }

        [JsonProperty("estimated_delivery_from")]
        public DateTime? EstimatedDeliveryFrom { get; set; }

        [JsonProperty("estimated_delivery_to")]
        public DateTime? EstimatedDeliveryTo { get; set; }

        [JsonProperty("original_estimated_delivery_from")]
        public DateTime? OriginalEstimatedDeliveryFrom { get; set; }

        [JsonProperty("is_pending")]
        public bool? IsPending { get; set; }

        [JsonProperty("current_branch_id")]
        public int? CurrentBranchId { get; set; }

        [JsonProperty("current_branch_name")]
        public string CurrentBranchName { get; set; }

        [JsonProperty("collection_branch_id")]
        public int? CollectionBranchId { get; set; }

        [JsonProperty("collection_branch_name")]
        public string CollectionBranchName { get; set; }

        [JsonProperty("delivery_branch_id")]
        public int? DeliveryBranchId { get; set; }

        [JsonProperty("delivery_branch_name")]
        public string DeliveryBranchName { get; set; }

        [JsonProperty("account_branch_id")]
        public int? AccountBranchId { get; set; }

        [JsonProperty("transient_branch_ids")]
        public string TransientBranchIds { get; set; }

        [JsonProperty("service_level_cut_off_time")]
        public DateTime? ServiceLevelCutOffTime { get; set; }

        [JsonProperty("collection_cutoff_override")]
        public bool? CollectionCutoffOverride { get; set; }

        [JsonProperty("all_charges_reversed")]
        public bool? AllChargesReversed { get; set; }

        [JsonProperty("total_note_count")]
        public int? TotalNoteCount { get; set; }

        [JsonProperty("external_note_count")]
        public int? ExternalNoteCount { get; set; }

        [JsonProperty("disable_new_charges")]
        public bool? DisableNewCharges { get; set; }

        [JsonProperty("mute_notifications")]
        public bool? MuteNotifications { get; set; }

        [JsonProperty("payment_intervention_status")]
        public string PaymentInterventionStatus { get; set; }

        [JsonProperty("pod_method")]
        public string PodMethod { get; set; }

        [JsonProperty("has_pod_files")]
        public bool? HasPodFiles { get; set; }

        [JsonProperty("has_been_invoiced")]
        public bool? HasBeenInvoiced { get; set; }

        [JsonProperty("tracking_events")]
        public string TrackingEvents { get; set; }

        [JsonProperty("collection_rescheduled")]
        public bool? CollectionRescheduled { get; set; }

        [JsonProperty("delivery_rescheduled")]
        public bool? DeliveryRescheduled { get; set; }

        [JsonProperty("rating_reference")]
        public string RatingReference { get; set; }

        [JsonProperty("has_rating")]
        public bool? HasRating { get; set; }
    }