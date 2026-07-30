import 'package:confios/core/di/injector.dart';
import 'package:confios/core/network/api_client.dart';
import 'package:confios/features/catalog/data/models/product.dart';
import 'package:confios/features/catalog/data/product_repository.dart';
import 'package:confios/features/catalog/presentation/pages/products_page.dart';
import 'package:confios/l10n/app_localizations.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

class _StubProductRepository extends ProductRepository {
  _StubProductRepository(this._products)
    : super(ApiClient.create('http://localhost'));

  final List<Product> _products;

  @override
  Future<List<Product>> list({String? locale}) async => _products;
}

void main() {
  Widget host(Widget child) => MaterialApp(
    localizationsDelegates: AppLocalizations.localizationsDelegates,
    supportedLocales: AppLocalizations.supportedLocales,
    home: Scaffold(body: child),
  );

  setUp(() async {
    await sl.reset();
    configureDependencies();
  });

  testWidgets('renders the toolbar and the products', (tester) async {
    sl.unregister<ProductRepository>();
    sl.registerLazySingleton<ProductRepository>(
      () => _StubProductRepository(const [
        Product(
          id: '1',
          name: 'Inyange Water 1.5L',
          sku: 'INY-15',
          priceAmount: 800,
          currencyCode: 'RWF',
          isActive: true,
        ),
      ]),
    );

    await tester.pumpWidget(host(const ProductsPage()));
    await tester.pumpAndSettle();

    expect(find.text('Add product'), findsOneWidget);
    expect(find.text('Inyange Water 1.5L'), findsOneWidget);
    expect(find.text('INY-15'), findsOneWidget);
  });

  testWidgets('shows the empty state with no products', (tester) async {
    sl.unregister<ProductRepository>();
    sl.registerLazySingleton<ProductRepository>(
      () => _StubProductRepository(const []),
    );

    await tester.pumpWidget(host(const ProductsPage()));
    await tester.pumpAndSettle();

    expect(find.text('No products yet'), findsOneWidget);
  });
}
