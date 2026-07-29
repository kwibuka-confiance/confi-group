import 'package:confios/app/app.dart';
import 'package:confios/core/di/injector.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  setUp(() async {
    await sl.reset();
    configureDependencies();
  });

  testWidgets('renders the sign-up form', (tester) async {
    await tester.pumpWidget(const ConfiOsApp());
    await tester.pumpAndSettle();

    // English is the default test locale.
    expect(find.text('Create your business'), findsOneWidget);
    expect(find.text('Create business'), findsOneWidget);
  });
}
