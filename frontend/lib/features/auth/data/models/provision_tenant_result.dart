/// The identifiers returned when a business is provisioned.
class ProvisionTenantResult {
  const ProvisionTenantResult({
    required this.tenantId,
    required this.branchId,
    required this.ownerUserId,
  });

  factory ProvisionTenantResult.fromJson(Map<String, dynamic> json) {
    return ProvisionTenantResult(
      tenantId: json['tenantId']?.toString() ?? '',
      branchId: json['branchId']?.toString() ?? '',
      ownerUserId: json['ownerUserId']?.toString() ?? '',
    );
  }

  final String tenantId;
  final String branchId;
  final String ownerUserId;
}
