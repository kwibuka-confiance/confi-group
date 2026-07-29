// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Kinyarwanda (`rw`).
class AppLocalizationsRw extends AppLocalizations {
  AppLocalizationsRw([String locale = 'rw']) : super(locale);

  @override
  String get appTitle => 'ConfiOS';

  @override
  String get signUpTitle => 'Shyiraho ubucuruzi bwawe';

  @override
  String get signUpSubtitle =>
      'Shyiraho ubucuruzi bwawe kuri ConfiOS mu munota umwe.';

  @override
  String get businessNameLabel => 'Izina ry\'ubucuruzi';

  @override
  String get slugLabel => 'Ikimenyetso cy\'ubucuruzi';

  @override
  String get slugHelper =>
      'Inyuguti nto, imibare n\'udukoni, urugero kwaconfi-depot';

  @override
  String get countryCodeLabel => 'Kode y\'igihugu';

  @override
  String get currencyCodeLabel => 'Kode y\'ifaranga';

  @override
  String get defaultLanguageLabel => 'Ururimi rusanzwe';

  @override
  String get timeZoneLabel => 'Isaha y\'akarere';

  @override
  String get firstBranchNameLabel => 'Izina ry\'ishami rya mbere';

  @override
  String get ownerFullNameLabel => 'Amazina yawe yombi';

  @override
  String get ownerEmailLabel => 'Imeyili yawe';

  @override
  String get ownerPasswordLabel => 'Ijambobanga';

  @override
  String get createButton => 'Shyiraho ubucuruzi';

  @override
  String get creatingButton => 'Birimo gukorwa…';

  @override
  String get successTitle => 'Ubucuruzi bwashyizweho';

  @override
  String successBody(String tenantId) {
    return 'Ubucuruzi bwawe bwiteguye. Umukiriya $tenantId ubu arakora.';
  }

  @override
  String get startOver => 'Shyiraho ubundi';

  @override
  String get fieldRequired => 'Iki gice kirakenewe';

  @override
  String get fieldInvalidFormat =>
      'Iyi ndangagaciro ntabwo iri mu buryo bukwiye';

  @override
  String get fieldTooShort => 'Iyi ndangagaciro ni ngufi cyane';

  @override
  String get fieldTooLong => 'Iyi ndangagaciro ni ndende cyane';

  @override
  String get errorSlugTaken => 'Icyo kimenyetso cy\'ubucuruzi cyarakoreshejwe.';

  @override
  String get errorNetwork =>
      'Ntabwo byashobotse kugera kuri seriveri. Reba umurongo maze wongere ugerageze.';

  @override
  String get errorUnexpected => 'Habaye ikibazo. Ongera ugerageze.';

  @override
  String get languageEnglish => 'Icyongereza';

  @override
  String get languageKinyarwanda => 'Ikinyarwanda';

  @override
  String get languageFrench => 'Igifaransa';
}
