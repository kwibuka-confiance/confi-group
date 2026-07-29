/// A product in the catalog.
class Product {
  const Product({
    required this.id,
    required this.name,
    required this.sku,
    required this.priceAmount,
    required this.currencyCode,
    required this.isActive,
  });

  factory Product.fromJson(Map<String, dynamic> json) {
    return Product(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      sku: json['sku']?.toString() ?? '',
      priceAmount: (json['priceAmount'] as num?)?.toDouble() ?? 0,
      currencyCode: json['currencyCode']?.toString() ?? '',
      isActive: json['isActive'] as bool? ?? true,
    );
  }

  final String id;
  final String name;
  final String sku;
  final double priceAmount;
  final String currencyCode;
  final bool isActive;
}
