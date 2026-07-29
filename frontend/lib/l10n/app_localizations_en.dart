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
  String get brandTagline => 'Run your whole business in one place';

  @override
  String get brandBlurb =>
      'Sales, inventory, customers and reports — together, in your language. Let\'s get you set up in a few simple steps.';

  @override
  String get brandPointCatalog => 'Track products and stock';

  @override
  String get brandPointSales => 'Record sales and payments';

  @override
  String get brandPointInsights => 'See how your business is doing';

  @override
  String stepOf(int current, int total) {
    return 'Step $current of $total';
  }

  @override
  String get stepBusinessLabel => 'Business';

  @override
  String get stepRegionLabel => 'Region';

  @override
  String get stepAccountLabel => 'Account';

  @override
  String get stepReviewLabel => 'Review';

  @override
  String get businessStepTitle => 'Your business';

  @override
  String get businessStepSubtitle =>
      'Tell us the name of the business you\'re setting up.';

  @override
  String get regionStepTitle => 'Region & currency';

  @override
  String get regionStepSubtitle =>
      'This sets how money, dates and language appear.';

  @override
  String get accountStepTitle => 'Your owner account';

  @override
  String get accountStepSubtitle =>
      'You\'ll use this to sign in and manage everything.';

  @override
  String get reviewStepTitle => 'Review & create';

  @override
  String get reviewStepSubtitle =>
      'Check the details below, then create your business.';

  @override
  String get businessNameLabel => 'Business name';

  @override
  String get businessNameHint => 'e.g. KwaConfi Depot';

  @override
  String get slugLabel => 'Business handle';

  @override
  String get slugHelper =>
      'Lowercase letters, numbers and hyphens. Used in your web address.';

  @override
  String get firstBranchNameLabel => 'First branch name';

  @override
  String get firstBranchNameHelper =>
      'The main location you sell from. You can add more later.';

  @override
  String get countryCodeLabel => 'Country code';

  @override
  String get countryCodeHelper => 'Two letters, e.g. RW';

  @override
  String get currencyCodeLabel => 'Currency code';

  @override
  String get currencyCodeHelper => 'Three letters, e.g. RWF';

  @override
  String get defaultLanguageLabel => 'Default language';

  @override
  String get timeZoneLabel => 'Time zone';

  @override
  String get ownerFullNameLabel => 'Your full name';

  @override
  String get ownerEmailLabel => 'Your email';

  @override
  String get ownerPasswordLabel => 'Password';

  @override
  String get passwordHelper => 'Use at least 12 characters.';

  @override
  String get showPassword => 'Show password';

  @override
  String get hidePassword => 'Hide password';

  @override
  String get backButton => 'Back';

  @override
  String get continueButton => 'Continue';

  @override
  String get createButton => 'Create business';

  @override
  String get creatingButton => 'Creating…';

  @override
  String get editAction => 'Edit';

  @override
  String get reviewBusinessHeading => 'Business';

  @override
  String get reviewRegionHeading => 'Region';

  @override
  String get reviewAccountHeading => 'Owner account';

  @override
  String get successTitle => 'Your business is ready';

  @override
  String successBody(String name) {
    return '$name is now set up on ConfiOS.';
  }

  @override
  String get successTenantHint => 'Keep this business ID for your records:';

  @override
  String get startOver => 'Create another business';

  @override
  String get copyTooltip => 'Copy';

  @override
  String get copiedMessage => 'Copied to clipboard';

  @override
  String get fieldRequired => 'This field is required';

  @override
  String get fieldInvalidFormat => 'This value is not in the expected format';

  @override
  String get fieldTooShort => 'This value is too short';

  @override
  String get fieldTooLong => 'This value is too long';

  @override
  String get emailInvalid => 'Enter a valid email address';

  @override
  String get slugInvalid => 'Use lowercase letters, numbers and hyphens only';

  @override
  String get passwordTooShort => 'Use at least 12 characters';

  @override
  String get errorSlugTaken =>
      'That business handle is already in use. Try another.';

  @override
  String get errorNetwork =>
      'Could not reach the server. Check your connection and try again.';

  @override
  String get errorUnexpected => 'Something went wrong. Please try again.';

  @override
  String get signInTitle => 'Welcome back';

  @override
  String get signInSubtitle => 'Sign in to your business.';

  @override
  String get handleFieldLabel => 'Business handle';

  @override
  String get signInHandleHelper =>
      'The handle you chose when creating the business.';

  @override
  String get emailFieldLabel => 'Email';

  @override
  String get passwordFieldLabel => 'Password';

  @override
  String get signInButton => 'Sign in';

  @override
  String get signingInButton => 'Signing in…';

  @override
  String get noBusinessPrompt => 'New to ConfiOS?';

  @override
  String get createBusinessLink => 'Create a business';

  @override
  String get haveBusinessPrompt => 'Already have a business?';

  @override
  String get signInLink => 'Sign in';

  @override
  String get goToSignIn => 'Sign in to your business';

  @override
  String homeWelcome(String name) {
    return 'Welcome, $name';
  }

  @override
  String homeSignedInTo(String business) {
    return 'You\'re signed in to $business.';
  }

  @override
  String get homeBusinessIdLabel => 'Business ID';

  @override
  String get homePermissionsLabel => 'Permissions';

  @override
  String homePermissionsCount(int count) {
    return '$count granted';
  }

  @override
  String get homeComingSoon =>
      'Your dashboard will grow here as Catalog, Inventory and Sales are built.';

  @override
  String get signOut => 'Sign out';

  @override
  String get themeTooltip => 'Theme';

  @override
  String get themeSystem => 'System';

  @override
  String get themeLight => 'Light';

  @override
  String get themeDark => 'Dark';

  @override
  String get languageEnglish => 'English';

  @override
  String get languageKinyarwanda => 'Kinyarwanda';

  @override
  String get languageFrench => 'French';
}
