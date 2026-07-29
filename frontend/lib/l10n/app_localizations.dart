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

  /// No description provided for @signUpTitle.
  ///
  /// In en, this message translates to:
  /// **'Create your business'**
  String get signUpTitle;

  /// No description provided for @signUpSubtitle.
  ///
  /// In en, this message translates to:
  /// **'Set up your business on ConfiOS in a minute.'**
  String get signUpSubtitle;

  /// No description provided for @businessNameLabel.
  ///
  /// In en, this message translates to:
  /// **'Business name'**
  String get businessNameLabel;

  /// No description provided for @slugLabel.
  ///
  /// In en, this message translates to:
  /// **'Business handle'**
  String get slugLabel;

  /// No description provided for @slugHelper.
  ///
  /// In en, this message translates to:
  /// **'Lowercase letters, numbers and hyphens, e.g. kwaconfi-depot'**
  String get slugHelper;

  /// No description provided for @countryCodeLabel.
  ///
  /// In en, this message translates to:
  /// **'Country code'**
  String get countryCodeLabel;

  /// No description provided for @currencyCodeLabel.
  ///
  /// In en, this message translates to:
  /// **'Currency code'**
  String get currencyCodeLabel;

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

  /// No description provided for @firstBranchNameLabel.
  ///
  /// In en, this message translates to:
  /// **'First branch name'**
  String get firstBranchNameLabel;

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

  /// No description provided for @successTitle.
  ///
  /// In en, this message translates to:
  /// **'Business created'**
  String get successTitle;

  /// No description provided for @successBody.
  ///
  /// In en, this message translates to:
  /// **'Your business is ready. Tenant {tenantId} is now active.'**
  String successBody(String tenantId);

  /// No description provided for @startOver.
  ///
  /// In en, this message translates to:
  /// **'Create another'**
  String get startOver;

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

  /// No description provided for @errorSlugTaken.
  ///
  /// In en, this message translates to:
  /// **'That business handle is already in use.'**
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
