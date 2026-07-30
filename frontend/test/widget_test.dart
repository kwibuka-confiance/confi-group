import 'package:confios/app/app.dart';
import 'package:confios/core/di/injector.dart';
import 'package:confios/core/network/api_client.dart';
import 'package:confios/features/auth/data/models/session.dart';
import 'package:confios/features/auth/data/session_store.dart';
import 'package:confios/features/catalog/data/models/product.dart';
import 'package:confios/features/catalog/data/product_repository.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Answers immediately so widget tests never reach the network.
class _FakeProductRepository extends ProductRepository {
  _FakeProductRepository() : super(ApiClient.create('http://localhost'));

  @override
  Future<List<Product>> list({String? locale}) async => const [];
}

void main() {
  setUp(() async {
    SharedPreferences.setMockInitialValues({});
    await sl.reset();
    configureDependencies();
    sl.unregister<ProductRepository>();
    sl.registerLazySingleton<ProductRepository>(_FakeProductRepository.new);
  });

  testWidgets('opens on the sign-in screen when signed out', (tester) async {
    await tester.pumpWidget(const ConfiOsApp());
    await tester.pumpAndSettle();

    // No persisted session, so the router redirects to the sign-in route.
    expect(find.text('Welcome back'), findsOneWidget);
    expect(find.text('Sign in'), findsOneWidget);
    expect(find.text('Create a business'), findsOneWidget);
  });

  testWidgets('restores a saved session into the dashboard shell', (tester) async {
    // Wide enough for the side navigation to render inline rather than in a drawer.
    tester.view.physicalSize = const Size(1400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final session = Session(
      accessToken: 'token',
      expiresAt: DateTime.now().add(const Duration(hours: 1)),
      userId: 'u1',
      tenantId: 't1',
      businessName: 'KwaConfi Depot',
      fullName: 'Confiance Owner',
      email: 'owner@kwaconfi.rw',
      permissions: const ['catalog.products.read'],
    );
    await sl<SessionStore>().save(session);
    final restored = await sl<SessionStore>().load();

    await tester.pumpWidget(ConfiOsApp(initialSession: restored));
    await tester.pumpAndSettle();

    // Redirected past sign-in into the shell: dashboard content plus the side
    // navigation and the signed-in business.
    expect(find.text('Welcome, Confiance Owner'), findsOneWidget);
    expect(find.text('KwaConfi Depot'), findsWidgets);
    expect(find.text('Products'), findsWidgets);
    expect(find.text('Welcome back'), findsNothing);
  });

  testWidgets('side navigation opens the products section', (tester) async {
    tester.view.physicalSize = const Size(1400, 900);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);

    final session = Session(
      accessToken: 'token',
      expiresAt: DateTime.now().add(const Duration(hours: 1)),
      userId: 'u1',
      tenantId: 't1',
      businessName: 'KwaConfi Depot',
      fullName: 'Confiance Owner',
      email: 'owner@kwaconfi.rw',
      permissions: const [],
    );

    await tester.pumpWidget(ConfiOsApp(initialSession: session));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Products').first);
    await tester.pumpAndSettle();

    // The products section renders inside the shell, not a blank pane. The fake
    // repository returns nothing, so the empty state is what should appear.
    expect(find.text('No products yet'), findsOneWidget);
    expect(find.text('Add product'), findsWidgets);
  });
}
