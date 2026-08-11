/**
 * Visible copy lives here, never inline in components, so every string can be
 * translated (English, Kinyarwanda and French are the MVP languages).
 */
export const locales = ['en', 'rw', 'fr'] as const;

export type Locale = (typeof locales)[number];

export const defaultLocale: Locale = 'en';

export function isLocale(value: string | undefined): value is Locale {
  return !!value && (locales as readonly string[]).includes(value);
}

const en = {
  appName: 'ConfiOS',
  nav: {
    groupMain: 'Main',
    groupOperations: 'Operations',
    dashboard: 'Dashboard',
    products: 'Products',
    inventory: 'Inventory',
    sales: 'Sales',
    reports: 'Reports',
    comingSoon: 'Soon',
    signOut: 'Sign out',
    toggleTheme: 'Toggle theme',
    openMenu: 'Open menu',
  },
  dashboard: {
    welcome: 'Welcome, {name}',
    signedInTo: "You're signed in to {business}.",
    statProducts: 'Products',
    statActive: 'Active',
    statCatalogValue: 'Catalog value',
    statPermissions: 'Permissions',
    currencyCount: '{count} currencies',
    recentlyAdded: 'Recently added',
    viewAll: 'View all',
    emptyHint: 'Add your first product and your catalog insights appear here.',
    quickActions: 'Quick actions',
  },
  products: {
    title: 'Products',
    searchPlaceholder: 'Search products or SKU',
    add: 'Add product',
    columnProduct: 'Product',
    columnSku: 'SKU',
    columnPrice: 'Price',
    columnStatus: 'Status',
    statusActive: 'Active',
    statusArchived: 'Archived',
    itemCount: '{count} items',
    itemCountOne: '1 item',
    empty: 'No products yet',
    emptyHint: 'Add your first product to get started.',
    noMatches: 'Nothing matches your search.',
  },
  auth: {
    signInTitle: 'Welcome back',
    signInSubtitle: 'Sign in to your business.',
    businessHandle: 'Business handle',
    businessHandleHint: 'The handle you chose when creating the business.',
    email: 'Email',
    password: 'Password',
    signIn: 'Sign in',
    signingIn: 'Signing in…',
    tagline: 'Run your whole business in one place',
    blurb:
      'Sales, inventory, customers and reports — together, in your language.',
    pointCatalog: 'Track products and stock',
    pointSales: 'Record sales and payments',
    pointInsights: 'See how your business is doing',
  },
  errors: {
    required: 'This field is required',
    unexpected: 'Something went wrong. Please try again.',
    network: 'Could not reach the server. Check your connection and try again.',
    loadFailed: 'Could not load your catalog.',
    retry: 'Retry',
  },
};

/**
 * Every dictionary matches the English shape, so a missing key is a type error.
 * English is deliberately not `as const`: values must stay `string` so a
 * translation is assignable, while the structure is still enforced.
 */
export type Dictionary = typeof en;

const rw: Dictionary = {
  appName: 'ConfiOS',
  nav: {
    groupMain: 'Ibanze',
    groupOperations: 'Ibikorwa',
    dashboard: 'Imbonerahamwe',
    products: 'Ibicuruzwa',
    inventory: 'Ububiko',
    sales: 'Ibyagurishijwe',
    reports: 'Raporo',
    comingSoon: 'Biraza',
    signOut: 'Sohoka',
    toggleTheme: 'Hindura insanganyamatsiko',
    openMenu: 'Fungura ibikubiye',
  },
  dashboard: {
    welcome: 'Murakaza neza, {name}',
    signedInTo: 'Winjiye muri {business}.',
    statProducts: 'Ibicuruzwa',
    statActive: 'Bikora',
    statCatalogValue: "Agaciro k'ibicuruzwa",
    statPermissions: 'Uburenganzira',
    currencyCount: 'Amafaranga {count}',
    recentlyAdded: 'Byongewemo vuba',
    viewAll: 'Reba byose',
    emptyHint: 'Ongeraho igicuruzwa cyawe cya mbere, incamake igaragare hano.',
    quickActions: 'Ibikorwa byihuse',
  },
  products: {
    title: 'Ibicuruzwa',
    searchPlaceholder: 'Shakisha igicuruzwa cyangwa SKU',
    add: 'Ongeraho igicuruzwa',
    columnProduct: 'Igicuruzwa',
    columnSku: 'SKU',
    columnPrice: 'Igiciro',
    columnStatus: 'Imiterere',
    statusActive: 'Kirakora',
    statusArchived: 'Cyabitswe',
    itemCount: 'Ibintu {count}',
    itemCountOne: 'Ikintu 1',
    empty: 'Nta bicuruzwa birahaba',
    emptyHint: 'Ongeraho igicuruzwa cyawe cya mbere kugira ngo utangire.',
    noMatches: "Nta kintu gihuye n'ubushakashatsi bwawe.",
  },
  auth: {
    signInTitle: 'Murakaza garuka',
    signInSubtitle: 'Injira mu bucuruzi bwawe.',
    businessHandle: "Ikimenyetso cy'ubucuruzi",
    businessHandleHint: 'Ikimenyetso wahisemo igihe washyiragaho ubucuruzi.',
    email: 'Imeyili',
    password: 'Ijambobanga',
    signIn: 'Injira',
    signingIn: 'Kwinjira…',
    tagline: 'Yobora ubucuruzi bwawe bwose ahantu hamwe',
    blurb: 'Kugurisha, ububiko, abakiriya na raporo — byose mu rurimi rwawe.',
    pointCatalog: "Kurikirana ibicuruzwa n'ububiko",
    pointSales: "Andika ibyagurishijwe n'ubwishyu",
    pointInsights: 'Reba uko ubucuruzi bwawe buhagaze',
  },
  errors: {
    required: 'Iki gice kirakenewe',
    unexpected: 'Habaye ikibazo. Ongera ugerageze.',
    network: 'Ntabwo byashobotse kugera kuri seriveri. Ongera ugerageze.',
    loadFailed: 'Ntibyashobotse gukura ibicuruzwa.',
    retry: 'Ongera ugerageze',
  },
};

const fr: Dictionary = {
  appName: 'ConfiOS',
  nav: {
    groupMain: 'Principal',
    groupOperations: 'Opérations',
    dashboard: 'Tableau de bord',
    products: 'Produits',
    inventory: 'Stock',
    sales: 'Ventes',
    reports: 'Rapports',
    comingSoon: 'Bientôt',
    signOut: 'Se déconnecter',
    toggleTheme: 'Changer de thème',
    openMenu: 'Ouvrir le menu',
  },
  dashboard: {
    welcome: 'Bienvenue, {name}',
    signedInTo: 'Vous êtes connecté à {business}.',
    statProducts: 'Produits',
    statActive: 'Actifs',
    statCatalogValue: 'Valeur du catalogue',
    statPermissions: 'Autorisations',
    currencyCount: '{count} devises',
    recentlyAdded: 'Ajoutés récemment',
    viewAll: 'Tout voir',
    emptyHint: 'Ajoutez votre premier produit pour voir les indicateurs ici.',
    quickActions: 'Actions rapides',
  },
  products: {
    title: 'Produits',
    searchPlaceholder: 'Rechercher un produit ou un SKU',
    add: 'Ajouter un produit',
    columnProduct: 'Produit',
    columnSku: 'SKU',
    columnPrice: 'Prix',
    columnStatus: 'Statut',
    statusActive: 'Actif',
    statusArchived: 'Archivé',
    itemCount: '{count} articles',
    itemCountOne: '1 article',
    empty: 'Aucun produit pour le moment',
    emptyHint: 'Ajoutez votre premier produit pour commencer.',
    noMatches: 'Aucun résultat ne correspond à votre recherche.',
  },
  auth: {
    signInTitle: 'Bon retour',
    signInSubtitle: 'Connectez-vous à votre entreprise.',
    businessHandle: "Identifiant de l'entreprise",
    businessHandleHint: "L'identifiant choisi lors de la création de l'entreprise.",
    email: 'E-mail',
    password: 'Mot de passe',
    signIn: 'Se connecter',
    signingIn: 'Connexion…',
    tagline: 'Gérez toute votre entreprise au même endroit',
    blurb: 'Ventes, stock, clients et rapports — réunis, dans votre langue.',
    pointCatalog: 'Suivez vos produits et votre stock',
    pointSales: 'Enregistrez ventes et paiements',
    pointInsights: 'Voyez comment se porte votre entreprise',
  },
  errors: {
    required: 'Ce champ est obligatoire',
    unexpected: "Une erreur s'est produite. Veuillez réessayer.",
    network: 'Impossible de joindre le serveur. Veuillez réessayer.',
    loadFailed: 'Impossible de charger votre catalogue.',
    retry: 'Réessayer',
  },
};

const dictionaries: Record<Locale, Dictionary> = { en, rw, fr };

export function getDictionary(locale: Locale = defaultLocale): Dictionary {
  return dictionaries[locale] ?? dictionaries[defaultLocale];
}

/** Substitutes `{name}` style placeholders. */
export function format(template: string, values: Record<string, string | number>): string {
  return template.replace(/\{(\w+)\}/g, (match, key: string) =>
    key in values ? String(values[key]) : match,
  );
}
