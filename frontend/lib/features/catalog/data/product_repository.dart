import '../../../core/network/api_client.dart';
import 'models/product.dart';

/// The Catalog module's client-side surface: listing and creating products.
class ProductRepository {
  const ProductRepository(this._client);

  final ApiClient _client;

  Future<List<Product>> list({String? locale}) async {
    final data = await _client.getList('/api/v1/products', locale: locale);
    return data
        .whereType<Map<String, dynamic>>()
        .map(Product.fromJson)
        .toList();
  }

  Future<String> create({
    required String name,
    required String sku,
    required double priceAmount,
    required String currencyCode,
    String? locale,
  }) async {
    final data = await _client.post(
      '/api/v1/products',
      body: {
        'name': name,
        'sku': sku,
        'priceAmount': priceAmount,
        'currencyCode': currencyCode,
      },
      locale: locale,
    );
    // The create endpoint returns the new id as its data payload.
    return data['id']?.toString() ?? '';
  }
}
