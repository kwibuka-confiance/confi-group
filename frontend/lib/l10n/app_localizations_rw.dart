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
  String get brandTagline => 'Yobora ubucuruzi bwawe bwose ahantu hamwe';

  @override
  String get brandBlurb =>
      'Kugurisha, ububiko, abakiriya na raporo — byose hamwe, mu rurimi rwawe. Reka tubifashe gutangira mu ntambwe nkeya zoroshye.';

  @override
  String get brandPointCatalog => 'Kurikirana ibicuruzwa n\'ububiko';

  @override
  String get brandPointSales => 'Andika ibyagurishijwe n\'ubwishyu';

  @override
  String get brandPointInsights => 'Reba uko ubucuruzi bwawe buhagaze';

  @override
  String stepOf(int current, int total) {
    return 'Intambwe ya $current kuri $total';
  }

  @override
  String get stepBusinessLabel => 'Ubucuruzi';

  @override
  String get stepRegionLabel => 'Akarere';

  @override
  String get stepAccountLabel => 'Konti';

  @override
  String get stepReviewLabel => 'Isuzuma';

  @override
  String get businessStepTitle => 'Ubucuruzi bwawe';

  @override
  String get businessStepSubtitle =>
      'Tubwire izina ry\'ubucuruzi urimo gushyiraho.';

  @override
  String get regionStepTitle => 'Akarere n\'ifaranga';

  @override
  String get regionStepSubtitle =>
      'Ibi bigena uko amafaranga, amatariki n\'ururimi bigaragara.';

  @override
  String get accountStepTitle => 'Konti yawe ya nyir\'ubucuruzi';

  @override
  String get accountStepSubtitle => 'Uzayikoresha winjira ukanayobora byose.';

  @override
  String get reviewStepTitle => 'Isuzuma no gushyiraho';

  @override
  String get reviewStepSubtitle =>
      'Reba amakuru hepfo, hanyuma ushyireho ubucuruzi bwawe.';

  @override
  String get businessNameLabel => 'Izina ry\'ubucuruzi';

  @override
  String get businessNameHint => 'urugero KwaConfi Depot';

  @override
  String get slugLabel => 'Ikimenyetso cy\'ubucuruzi';

  @override
  String get slugHelper =>
      'Inyuguti nto, imibare n\'udukoni. Bikoreshwa muri aderesi yawe kuri interineti.';

  @override
  String get firstBranchNameLabel => 'Izina ry\'ishami rya mbere';

  @override
  String get firstBranchNameHelper =>
      'Aho ugurishiriza ahanini. Ushobora kongeraho andi nyuma.';

  @override
  String get countryCodeLabel => 'Kode y\'igihugu';

  @override
  String get countryCodeHelper => 'Inyuguti ebyiri, urugero RW';

  @override
  String get currencyCodeLabel => 'Kode y\'ifaranga';

  @override
  String get currencyCodeHelper => 'Inyuguti eshatu, urugero RWF';

  @override
  String get defaultLanguageLabel => 'Ururimi rusanzwe';

  @override
  String get timeZoneLabel => 'Isaha y\'akarere';

  @override
  String get ownerFullNameLabel => 'Amazina yawe yombi';

  @override
  String get ownerEmailLabel => 'Imeyili yawe';

  @override
  String get ownerPasswordLabel => 'Ijambobanga';

  @override
  String get passwordHelper => 'Koresha byibuze inyuguti 12.';

  @override
  String get showPassword => 'Erekana ijambobanga';

  @override
  String get hidePassword => 'Hisha ijambobanga';

  @override
  String get backButton => 'Subira inyuma';

  @override
  String get continueButton => 'Komeza';

  @override
  String get createButton => 'Shyiraho ubucuruzi';

  @override
  String get creatingButton => 'Birimo gukorwa…';

  @override
  String get editAction => 'Hindura';

  @override
  String get reviewBusinessHeading => 'Ubucuruzi';

  @override
  String get reviewRegionHeading => 'Akarere';

  @override
  String get reviewAccountHeading => 'Konti ya nyir\'ubucuruzi';

  @override
  String get successTitle => 'Ubucuruzi bwawe bwiteguye';

  @override
  String successBody(String name) {
    return '$name ubu bwashyizweho kuri ConfiOS.';
  }

  @override
  String get successTenantHint =>
      'Bika iyi nomero y\'ubucuruzi mu nyandiko zawe:';

  @override
  String get startOver => 'Shyiraho ubundi bucuruzi';

  @override
  String get copyTooltip => 'Koporora';

  @override
  String get copiedMessage => 'Byakoporowe';

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
  String get emailInvalid => 'Andika imeyili yemewe';

  @override
  String get slugInvalid => 'Koresha inyuguti nto, imibare n\'udukoni gusa';

  @override
  String get passwordTooShort => 'Koresha byibuze inyuguti 12';

  @override
  String get errorSlugTaken =>
      'Icyo kimenyetso cy\'ubucuruzi cyarakoreshejwe. Gerageza ikindi.';

  @override
  String get errorNetwork =>
      'Ntabwo byashobotse kugera kuri seriveri. Reba umurongo maze wongere ugerageze.';

  @override
  String get errorUnexpected => 'Habaye ikibazo. Ongera ugerageze.';

  @override
  String get signInTitle => 'Murakaza garuka';

  @override
  String get signInSubtitle => 'Injira mu bucuruzi bwawe.';

  @override
  String get handleFieldLabel => 'Ikimenyetso cy\'ubucuruzi';

  @override
  String get signInHandleHelper =>
      'Ikimenyetso wahisemo igihe washyiragaho ubucuruzi.';

  @override
  String get emailFieldLabel => 'Imeyili';

  @override
  String get passwordFieldLabel => 'Ijambobanga';

  @override
  String get signInButton => 'Injira';

  @override
  String get signingInButton => 'Kwinjira…';

  @override
  String get noBusinessPrompt => 'Uri mushya kuri ConfiOS?';

  @override
  String get createBusinessLink => 'Shyiraho ubucuruzi';

  @override
  String get haveBusinessPrompt => 'Usanzwe ufite ubucuruzi?';

  @override
  String get signInLink => 'Injira';

  @override
  String get goToSignIn => 'Injira mu bucuruzi bwawe';

  @override
  String homeWelcome(String name) {
    return 'Murakaza neza, $name';
  }

  @override
  String homeSignedInTo(String business) {
    return 'Winjiye muri $business.';
  }

  @override
  String get homeBusinessIdLabel => 'Nomero y\'ubucuruzi';

  @override
  String get homePermissionsLabel => 'Uburenganzira';

  @override
  String homePermissionsCount(int count) {
    return '$count yatanzwe';
  }

  @override
  String get homeComingSoon =>
      'Imbonerahamwe yawe izakura hano uko Ibicuruzwa, Ububiko n\'Ibyagurishijwe bizubakwa.';

  @override
  String get signOut => 'Sohoka';

  @override
  String get navGroupMain => 'Ibanze';

  @override
  String get navGroupOperations => 'Ibikorwa';

  @override
  String get searchProductsHint => 'Shakisha igicuruzwa cyangwa SKU';

  @override
  String get columnProduct => 'Igicuruzwa';

  @override
  String get columnSku => 'SKU';

  @override
  String get columnPrice => 'Igiciro';

  @override
  String get columnStatus => 'Imiterere';

  @override
  String get statusActive => 'Kirakora';

  @override
  String get statusArchived => 'Cyabitswe';

  @override
  String get noSearchResults => 'Nta kintu gihuye n\'ubushakashatsi bwawe.';

  @override
  String get clearSearch => 'Siba ubushakashatsi';

  @override
  String itemCount(int count) {
    String _temp0 = intl.Intl.pluralLogic(
      count,
      locale: localeName,
      other: 'Ibintu $count',
      one: 'Ikintu 1',
    );
    return '$_temp0';
  }

  @override
  String get navDashboard => 'Imbonerahamwe';

  @override
  String get navProducts => 'Ibicuruzwa';

  @override
  String get navInventory => 'Ububiko';

  @override
  String get navSales => 'Ibyagurishijwe';

  @override
  String get navReports => 'Raporo';

  @override
  String get navSettings => 'Igenamiterere';

  @override
  String get comingSoonBadge => 'Biraza';

  @override
  String get menuTooltip => 'Ibikubiye';

  @override
  String get dashboardTitle => 'Imbonerahamwe';

  @override
  String get statProducts => 'Ibicuruzwa';

  @override
  String get statActiveProducts => 'Bikora';

  @override
  String get statCatalogValue => 'Agaciro k\'ibicuruzwa';

  @override
  String get statPermissions => 'Uburenganzira';

  @override
  String currencyCount(int count) {
    return 'Amafaranga $count';
  }

  @override
  String get recentlyAddedTitle => 'Byongewemo vuba';

  @override
  String get viewAllAction => 'Reba byose';

  @override
  String get dashboardEmptyHint =>
      'Ongeraho igicuruzwa cyawe cya mbere, hanyuma incamake y\'ibicuruzwa igaragare hano.';

  @override
  String get quickActionsTitle => 'Ibikorwa byihuse';

  @override
  String get productsTitle => 'Ibicuruzwa';

  @override
  String get openCatalog => 'Fungura ibicuruzwa';

  @override
  String get addProduct => 'Ongeraho igicuruzwa';

  @override
  String get productNameLabel => 'Izina ry\'igicuruzwa';

  @override
  String get productSkuLabel => 'SKU';

  @override
  String get productPriceLabel => 'Igiciro';

  @override
  String get createProductButton => 'Shyiraho igicuruzwa';

  @override
  String get noProductsTitle => 'Nta bicuruzwa birahaba';

  @override
  String get noProductsHint =>
      'Ongeraho igicuruzwa cyawe cya mbere kugira ngo utangire.';

  @override
  String get productsLoadError => 'Ntibyashobotse gukura ibicuruzwa.';

  @override
  String get retryButton => 'Ongera ugerageze';

  @override
  String get themeTooltip => 'Insanganyamatsiko';

  @override
  String get themeSystem => 'Iya sisitemu';

  @override
  String get themeLight => 'Umucyo';

  @override
  String get themeDark => 'Umwijima';

  @override
  String get languageEnglish => 'Icyongereza';

  @override
  String get languageKinyarwanda => 'Ikinyarwanda';

  @override
  String get languageFrench => 'Igifaransa';
}
