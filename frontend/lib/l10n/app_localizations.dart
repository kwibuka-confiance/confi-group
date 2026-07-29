import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_fr.dart';
import 'app_localizations_rw.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('fr'),
    Locale('rw'),
  ];

  /// No description provided for @appTitle.
  ///
  /// In en, this message translates to:
  /// **'ConfiOS'**
  String get appTitle;

  /// No description provided for @brandTagline.
  ///
  /// In en, this message translates to:
  /// **'Run your whole business in one place'**
  String get brandTagline;

  /// No description provided for @brandBlurb.
  ///
  /// In en, this message translates to:
  /// **'Sales, inventory, customers and reports — together, in your language. Let\'s get you set up in a few simple steps.'**
  String get brandBlurb;

  /// No description provided for @brandPointCatalog.
  ///
  /// In en, this message translates to:
  /// **'Track products and stock'**
  String get brandPointCatalog;

  /// No description provided for @brandPointSales.
  ///
  /// In en, this message translates to:
  /// **'Record sales and payments'**
  String get brandPointSales;

  /// No description provided for @brandPointInsights.
  ///
  /// In en, this message translates to:
  /// **'See how your business is doing'**
  String get brandPointInsights;

  /// No description provided for @stepOf.
  ///
  /// In en, this message translates to:
  /// **'Step {current} of {total}'**
  String stepOf(int current, int total);

  /// No description provided for @stepBusinessLabel.
  ///
  /// In en, this message translates to:
  /// **'Business'**
  String get stepBusinessLabel;

  /// No description provided for @stepRegionLabel.
  ///
  /// In en, this message translates to:
  /// **'Region'**
  String get stepRegionLabel;

  /// No description provided for @stepAccountLabel.
  ///
  /// In en, this message translates to:
  /// **'Account'**
  String get stepAccountLabel;

  /// No description provided for @stepReviewLabel.
  ///
  /// In en, this message translates to:
  /// **'Review'**
  String get stepReviewLabel;

  /// No description provided for @businessStepTitle.
  ///
  /// In en, this message translates to:
  /// **'Your business'**
  String get businessStepTitle;

  /// No description provided for @businessStepSubtitle.
  ///
  /// In en, this message translates to:
  /// **'Tell us the name of the business you\'re setting up.'**
  String get businessStepSubtitle;

  /// No description provided for @regionStepTitle.
  ///
  /// In en, this message translates to:
  /// **'Region & currency'**
  String get regionStepTitle;

  /// No description provided for @regionStepSubtitle.
  ///
  /// In en, this message translates to:
  /// **'This sets how money, dates and language appear.'**
  String get regionStepSubtitle;

  /// No description provided for @accountStepTitle.
  ///
  /// In en, this message translates to:
  /// **'Your owner account'**
  String get accountStepTitle;

  /// No description provided for @accountStepSubtitle.
  ///
  /// In en, this message translates to:
  /// **'You\'ll use this to sign in and manage everything.'**
  String get accountStepSubtitle;

  /// No description provided for @reviewStepTitle.
  ///
  /// In en, this message translates to:
  /// **'Review & create'**
  String get reviewStepTitle;

  /// No description provided for @reviewStepSubtitle.
  ///
  /// In en, this message translates to:
  /// **'Check the details below, then create your business.'**
  String get reviewStepSubtitle;

  /// No description provided for @businessNameLabel.
  ///
  /// In en, this message translates to:
  /// **'Business name'**
  String get businessNameLabel;

  /// No description provided for @businessNameHint.
  ///
  /// In en, this message translates to:
  /// **'e.g. KwaConfi Depot'**
  String get businessNameHint;

  /// No description provided for @slugLabel.
  ///
  /// In en, this message translates to:
  /// **'Business handle'**
  String get slugLabel;

  /// No description provided for @slugHelper.
  ///
  /// In en, this message translates to:
  /// **'Lowercase letters, numbers and hyphens. Used in your web address.'**
  String get slugHelper;

  /// No description provided for @firstBranchNameLabel.
  ///
  /// In en, this message translates to:
  /// **'First branch name'**
  String get firstBranchNameLabel;

  /// No description provided for @firstBranchNameHelper.
  ///
  /// In en, this message translates to:
  /// **'The main location you sell from. You can add more later.'**
  String get firstBranchNameHelper;

  /// No description provided for @countryCodeLabel.
  ///
  /// In en, this message translates to:
  /// **'Country code'**
  String get countryCodeLabel;

  /// No description provided for @countryCodeHelper.
  ///
  /// In en, this message translates to:
  /// **'Two letters, e.g. RW'**
  String get countryCodeHelper;

  /// No description provided for @currencyCodeLabel.
  ///
  /// In en, this message translates to:
  /// **'Currency code'**
  String get currencyCodeLabel;

  /// No description provided for @currencyCodeHelper.
  ///
  /// In en, this message translates to:
  /// **'Three letters, e.g. RWF'**
  String get currencyCodeHelper;

  /// No description provided for @defaultLanguageLabel.
  ///
  /// In en, this message translates to:
  /// **'Default language'**
  String get defaultLanguageLabel;

  /// No description provided for @timeZoneLabel.
  ///
  /// In en, this message translates to:
  /// **'Time zone'**
  String get timeZoneLabel;

  /// No description provided for @ownerFullNameLabel.
  ///
  /// In en, this message translates to:
  /// **'Your full name'**
  String get ownerFullNameLabel;

  /// No description provided for @ownerEmailLabel.
  ///
  /// In en, this message translates to:
  /// **'Your email'**
  String get ownerEmailLabel;

  /// No description provided for @ownerPasswordLabel.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get ownerPasswordLabel;

  /// No description provided for @passwordHelper.
  ///
  /// In en, this message translates to:
  /// **'Use at least 12 characters.'**
  String get passwordHelper;

  /// No description provided for @showPassword.
  ///
  /// In en, this message translates to:
  /// **'Show password'**
  String get showPassword;

  /// No description provided for @hidePassword.
  ///
  /// In en, this message translates to:
  /// **'Hide password'**
  String get hidePassword;

  /// No description provided for @backButton.
  ///
  /// In en, this message translates to:
  /// **'Back'**
  String get backButton;

  /// No description provided for @continueButton.
  ///
  /// In en, this message translates to:
  /// **'Continue'**
  String get continueButton;

  /// No description provided for @createButton.
  ///
  /// In en, this message translates to:
  /// **'Create business'**
  String get createButton;

  /// No description provided for @creatingButton.
  ///
  /// In en, this message translates to:
  /// **'Creating…'**
  String get creatingButton;

  /// No description provided for @editAction.
  ///
  /// In en, this message translates to:
  /// **'Edit'**
  String get editAction;

  /// No description provided for @reviewBusinessHeading.
  ///
  /// In en, this message translates to:
  /// **'Business'**
  String get reviewBusinessHeading;

  /// No description provided for @reviewRegionHeading.
  ///
  /// In en, this message translates to:
  /// **'Region'**
  String get reviewRegionHeading;

  /// No description provided for @reviewAccountHeading.
  ///
  /// In en, this message translates to:
  /// **'Owner account'**
  String get reviewAccountHeading;

  /// No description provided for @successTitle.
  ///
  /// In en, this message translates to:
  /// **'Your business is ready'**
  String get successTitle;

  /// No description provided for @successBody.
  ///
  /// In en, this message translates to:
  /// **'{name} is now set up on ConfiOS.'**
  String successBody(String name);

  /// No description provided for @successTenantHint.
  ///
  /// In en, this message translates to:
  /// **'Keep this business ID for your records:'**
  String get successTenantHint;

  /// No description provided for @startOver.
  ///
  /// In en, this message translates to:
  /// **'Create another business'**
  String get startOver;

  /// No description provided for @copyTooltip.
  ///
  /// In en, this message translates to:
  /// **'Copy'**
  String get copyTooltip;

  /// No description provided for @copiedMessage.
  ///
  /// In en, this message translates to:
  /// **'Copied to clipboard'**
  String get copiedMessage;

  /// No description provided for @fieldRequired.
  ///
  /// In en, this message translates to:
  /// **'This field is required'**
  String get fieldRequired;

  /// No description provided for @fieldInvalidFormat.
  ///
  /// In en, this message translates to:
  /// **'This value is not in the expected format'**
  String get fieldInvalidFormat;

  /// No description provided for @fieldTooShort.
  ///
  /// In en, this message translates to:
  /// **'This value is too short'**
  String get fieldTooShort;

  /// No description provided for @fieldTooLong.
  ///
  /// In en, this message translates to:
  /// **'This value is too long'**
  String get fieldTooLong;

  /// No description provided for @emailInvalid.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid email address'**
  String get emailInvalid;

  /// No description provided for @slugInvalid.
  ///
  /// In en, this message translates to:
  /// **'Use lowercase letters, numbers and hyphens only'**
  String get slugInvalid;

  /// No description provided for @passwordTooShort.
  ///
  /// In en, this message translates to:
  /// **'Use at least 12 characters'**
  String get passwordTooShort;

  /// No description provided for @errorSlugTaken.
  ///
  /// In en, this message translates to:
  /// **'That business handle is already in use. Try another.'**
  String get errorSlugTaken;

  /// No description provided for @errorNetwork.
  ///
  /// In en, this message translates to:
  /// **'Could not reach the server. Check your connection and try again.'**
  String get errorNetwork;

  /// No description provided for @errorUnexpected.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong. Please try again.'**
  String get errorUnexpected;

  /// No description provided for @signInTitle.
  ///
  /// In en, this message translates to:
  /// **'Welcome back'**
  String get signInTitle;

  /// No description provided for @signInSubtitle.
  ///
  /// In en, this message translates to:
  /// **'Sign in to your business.'**
  String get signInSubtitle;

  /// No description provided for @handleFieldLabel.
  ///
  /// In en, this message translates to:
  /// **'Business handle'**
  String get handleFieldLabel;

  /// No description provided for @signInHandleHelper.
  ///
  /// In en, this message translates to:
  /// **'The handle you chose when creating the business.'**
  String get signInHandleHelper;

  /// No description provided for @emailFieldLabel.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get emailFieldLabel;

  /// No description provided for @passwordFieldLabel.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get passwordFieldLabel;

  /// No description provided for @signInButton.
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get signInButton;

  /// No description provided for @signingInButton.
  ///
  /// In en, this message translates to:
  /// **'Signing in…'**
  String get signingInButton;

  /// No description provided for @noBusinessPrompt.
  ///
  /// In en, this message translates to:
  /// **'New to ConfiOS?'**
  String get noBusinessPrompt;

  /// No description provided for @createBusinessLink.
  ///
  /// In en, this message translates to:
  /// **'Create a business'**
  String get createBusinessLink;

  /// No description provided for @haveBusinessPrompt.
  ///
  /// In en, this message translates to:
  /// **'Already have a business?'**
  String get haveBusinessPrompt;

  /// No description provided for @signInLink.
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get signInLink;

  /// No description provided for @goToSignIn.
  ///
  /// In en, this message translates to:
  /// **'Sign in to your business'**
  String get goToSignIn;

  /// No description provided for @homeWelcome.
  ///
  /// In en, this message translates to:
  /// **'Welcome, {name}'**
  String homeWelcome(String name);

  /// No description provided for @homeSignedInTo.
  ///
  /// In en, this message translates to:
  /// **'You\'re signed in to {business}.'**
  String homeSignedInTo(String business);

  /// No description provided for @homeBusinessIdLabel.
  ///
  /// In en, this message translates to:
  /// **'Business ID'**
  String get homeBusinessIdLabel;

  /// No description provided for @homePermissionsLabel.
  ///
  /// In en, this message translates to:
  /// **'Permissions'**
  String get homePermissionsLabel;

  /// No description provided for @homePermissionsCount.
  ///
  /// In en, this message translates to:
  /// **'{count} granted'**
  String homePermissionsCount(int count);

  /// No description provided for @homeComingSoon.
  ///
  /// In en, this message translates to:
  /// **'Your dashboard will grow here as Catalog, Inventory and Sales are built.'**
  String get homeComingSoon;

  /// No description provided for @signOut.
  ///
  /// In en, this message translates to:
  /// **'Sign out'**
  String get signOut;

  /// No description provided for @productsTitle.
  ///
  /// In en, this message translates to:
  /// **'Products'**
  String get productsTitle;

  /// No description provided for @openCatalog.
  ///
  /// In en, this message translates to:
  /// **'Open catalog'**
  String get openCatalog;

  /// No description provided for @addProduct.
  ///
  /// In en, this message translates to:
  /// **'Add product'**
  String get addProduct;

  /// No description provided for @productNameLabel.
  ///
  /// In en, this message translates to:
  /// **'Product name'**
  String get productNameLabel;

  /// No description provided for @productSkuLabel.
  ///
  /// In en, this message translates to:
  /// **'SKU'**
  String get productSkuLabel;

  /// No description provided for @productPriceLabel.
  ///
  /// In en, this message translates to:
  /// **'Price'**
  String get productPriceLabel;

  /// No description provided for @createProductButton.
  ///
  /// In en, this message translates to:
  /// **'Create product'**
  String get createProductButton;

  /// No description provided for @noProductsTitle.
  ///
  /// In en, this message translates to:
  /// **'No products yet'**
  String get noProductsTitle;

  /// No description provided for @noProductsHint.
  ///
  /// In en, this message translates to:
  /// **'Add your first product to get started.'**
  String get noProductsHint;

  /// No description provided for @productsLoadError.
  ///
  /// In en, this message translates to:
  /// **'Could not load products.'**
  String get productsLoadError;

  /// No description provided for @retryButton.
  ///
  /// In en, this message translates to:
  /// **'Retry'**
  String get retryButton;

  /// No description provided for @themeTooltip.
  ///
  /// In en, this message translates to:
  /// **'Theme'**
  String get themeTooltip;

  /// No description provided for @themeSystem.
  ///
  /// In en, this message translates to:
  /// **'System'**
  String get themeSystem;

  /// No description provided for @themeLight.
  ///
  /// In en, this message translates to:
  /// **'Light'**
  String get themeLight;

  /// No description provided for @themeDark.
  ///
  /// In en, this message translates to:
  /// **'Dark'**
  String get themeDark;

  /// No description provided for @languageEnglish.
  ///
  /// In en, this message translates to:
  /// **'English'**
  String get languageEnglish;

  /// No description provided for @languageKinyarwanda.
  ///
  /// In en, this message translates to:
  /// **'Kinyarwanda'**
  String get languageKinyarwanda;

  /// No description provided for @languageFrench.
  ///
  /// In en, this message translates to:
  /// **'French'**
  String get languageFrench;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'fr', 'rw'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'fr':
      return AppLocalizationsFr();
    case 'rw':
      return AppLocalizationsRw();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
