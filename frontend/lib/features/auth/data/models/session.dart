/// An authenticated session: the access token plus the identity behind it.
class Session {
  const Session({
    required this.accessToken,
    required this.expiresAt,
    required this.userId,
    required this.tenantId,
    required this.businessName,
    required this.fullName,
    required this.email,
    required this.permissions,
  });

  factory Session.fromJson(Map<String, dynamic> json) {
    return Session(
      accessToken: json['accessToken']?.toString() ?? '',
      expiresAt:
          DateTime.tryParse(json['expiresAt']?.toString() ?? '') ?? DateTime.now(),
      userId: json['userId']?.toString() ?? '',
      tenantId: json['tenantId']?.toString() ?? '',
      businessName: json['businessName']?.toString() ?? '',
      fullName: json['fullName']?.toString() ?? '',
      email: json['email']?.toString() ?? '',
      permissions:
          (json['permissions'] as List?)?.map((e) => e.toString()).toList() ??
          const [],
    );
  }

  final String accessToken;
  final DateTime expiresAt;
  final String userId;
  final String tenantId;
  final String businessName;
  final String fullName;
  final String email;
  final List<String> permissions;

  bool get isExpired => DateTime.now().isAfter(expiresAt);

  Map<String, dynamic> toJson() => {
    'accessToken': accessToken,
    'expiresAt': expiresAt.toIso8601String(),
    'userId': userId,
    'tenantId': tenantId,
    'businessName': businessName,
    'fullName': fullName,
    'email': email,
    'permissions': permissions,
  };
}
