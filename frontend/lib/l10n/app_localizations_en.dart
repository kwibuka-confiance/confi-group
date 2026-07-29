// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'ConfiOS';

  @override
  String get signUpTitle => 'Create your business';

  @override
  String get signUpSubtitle => 'Set up your business on ConfiOS in a minute.';

  @override
  String get businessNameLabel => 'Business name';

  @override
  String get slugLabel => 'Business handle';

  @override
  String get slugHelper =>
      'Lowercase letters, numbers and hyphens, e.g. kwaconfi-depot';

  @override
  String get countryCodeLabel => 'Country code';

  @override
  String get currencyCodeLabel => 'Currency code';

  @override
  String get defaultLanguageLabel => 'Default language';

  @override
  String get timeZoneLabel => 'Time zone';

  @override
  String get firstBranchNameLabel => 'First branch name';

  @override
  String get ownerFullNameLabel => 'Your full name';

  @override
  String get ownerEmailLabel => 'Your email';

  @override
  String get ownerPasswordLabel => 'Password';

  @override
  String get createButton => 'Create business';

  @override
  String get creatingButton => 'Creating…';

  @override
  String get successTitle => 'Business created';

  @override
  String successBody(String tenantId) {
    return 'Your business is ready. Tenant $tenantId is now active.';
  }

  @override
  String get startOver => 'Create another';

  @override
  String get fieldRequired => 'This field is required';

  @override
  String get fieldInvalidFormat => 'This value is not in the expected format';

  @override
  String get fieldTooShort => 'This value is too short';

  @override
  String get fieldTooLong => 'This value is too long';

  @override
  String get errorSlugTaken => 'That business handle is already in use.';

  @override
  String get errorNetwork =>
      'Could not reach the server. Check your connection and try again.';

  @override
  String get errorUnexpected => 'Something went wrong. Please try again.';

  @override
  String get languageEnglish => 'English';

  @override
  String get languageKinyarwanda => 'Kinyarwanda';

  @override
  String get languageFrench => 'French';
}
