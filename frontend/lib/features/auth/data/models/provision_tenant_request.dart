/// Sign-up payload for `POST /api/v1/tenants`. Field names match the backend
/// contract exactly so [toJson] maps straight onto the wire format.
class ProvisionTenantRequest {
  const ProvisionTenantRequest({
    required this.name,
    required this.slug,
    required this.countryCode,
    required this.currencyCode,
    required this.defaultLanguage,
    required this.timeZoneId,
    required this.ownerEmail,
    required this.ownerFullName,
    required this.ownerPassword,
    required this.firstBranchName,
  });

  final String name;
  final String slug;
  final String countryCode;
  final String currencyCode;
  final String defaultLanguage;
  final String timeZoneId;
  final String ownerEmail;
  final String ownerFullName;
  final String ownerPassword;
  final String firstBranchName;

  Map<String, dynamic> toJson() => {
    'name': name,
    'slug': slug,
    'countryCode': countryCode,
    'currencyCode': currencyCode,
    'defaultLanguage': defaultLanguage,
    'timeZoneId': timeZoneId,
    'ownerEmail': ownerEmail,
    'ownerFullName': ownerFullName,
    'ownerPassword': ownerPassword,
    'firstBranchName': firstBranchName,
  };
}
