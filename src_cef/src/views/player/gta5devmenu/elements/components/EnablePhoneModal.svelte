<script>
  import { createEventDispatcher } from 'svelte';
  import SVGComponent from "../components/SVGComponent.svelte";

  const dispatch = createEventDispatcher();

  let phoneNumber = '';
  let verificationCode = '';
  let country = 'RU';
  let step = 1; // 1: enter phone, 2: enter code
  let isLoadingSendCode = false;
  let phoneError = '';
  let codeError = '';
  let showCountrySelect = false;
  let searchQuery = '';

  const countries = [
    { iso2: 'ad', dialCode: '+376', name: 'Андорра' },
    { iso2: 'ae', dialCode: '+971', name: 'ОАЭ' },
    { iso2: 'af', dialCode: '+93', name: 'Афганистан' },
    { iso2: 'ag', dialCode: '+1', name: 'Антигуа и Барбуда' },
    { iso2: 'ai', dialCode: '+1', name: 'Ангилья' },
    { iso2: 'al', dialCode: '+355', name: 'Албания' },
    { iso2: 'am', dialCode: '+374', name: 'Армения' },
    { iso2: 'ao', dialCode: '+244', name: 'Ангола' },
    { iso2: 'ar', dialCode: '+54', name: 'Аргентина' },
    { iso2: 'as', dialCode: '+1', name: 'Американское Самоа' },
    { iso2: 'at', dialCode: '+43', name: 'Австрия' },
    { iso2: 'au', dialCode: '+61', name: 'Австралия' },
    { iso2: 'aw', dialCode: '+297', name: 'Аруба' },
    { iso2: 'az', dialCode: '+994', name: 'Азербайджан' },
    { iso2: 'ba', dialCode: '+387', name: 'Босния и Герцеговина' },
    { iso2: 'bb', dialCode: '+1', name: 'Барбадос' },
    { iso2: 'bd', dialCode: '+880', name: 'Бангладеш' },
    { iso2: 'be', dialCode: '+32', name: 'Бельгия' },
    { iso2: 'bf', dialCode: '+226', name: 'Буркина-Фасо' },
    { iso2: 'bg', dialCode: '+359', name: 'Болгария' },
    { iso2: 'bh', dialCode: '+973', name: 'Бахрейн' },
    { iso2: 'bi', dialCode: '+257', name: 'Бурунди' },
    { iso2: 'bj', dialCode: '+229', name: 'Бенин' },
    { iso2: 'bl', dialCode: '+590', name: 'Сент-Барт' },
    { iso2: 'bm', dialCode: '+1', name: 'Бермуды' },
    { iso2: 'bn', dialCode: '+673', name: 'Бруней' },
    { iso2: 'bo', dialCode: '+591', name: 'Боливия' },
    { iso2: 'bq', dialCode: '+599', name: 'Карибские Нидерланды' },
    { iso2: 'br', dialCode: '+55', name: 'Бразилия' },
    { iso2: 'bs', dialCode: '+1', name: 'Багамы' },
    { iso2: 'bt', dialCode: '+975', name: 'Бутан' },
    { iso2: 'bw', dialCode: '+267', name: 'Ботсвана' },
    { iso2: 'by', dialCode: '+375', name: 'Беларусь' },
    { iso2: 'bz', dialCode: '+501', name: 'Белиз' },
    { iso2: 'ca', dialCode: '+1', name: 'Канада' },
    { iso2: 'cc', dialCode: '+61', name: 'Кокосовые острова' },
    { iso2: 'cd', dialCode: '+243', name: 'Демократическая Республика Конго' },
    { iso2: 'cf', dialCode: '+236', name: 'Центральноафриканская Республика' },
    { iso2: 'cg', dialCode: '+242', name: 'Конго' },
    { iso2: 'ch', dialCode: '+41', name: 'Швейцария' },
    { iso2: 'ci', dialCode: '+225', name: 'Кот-д\'Ивуар' },
    { iso2: 'ck', dialCode: '+682', name: 'Острова Кука' },
    { iso2: 'cl', dialCode: '+56', name: 'Чили' },
    { iso2: 'cm', dialCode: '+237', name: 'Камерун' },
    { iso2: 'cn', dialCode: '+86', name: 'Китай' },
    { iso2: 'co', dialCode: '+57', name: 'Колумбия' },
    { iso2: 'cr', dialCode: '+506', name: 'Коста-Рика' },
    { iso2: 'cu', dialCode: '+53', name: 'Куба' },
    { iso2: 'cv', dialCode: '+238', name: 'Кабо-Верде' },
    { iso2: 'cw', dialCode: '+599', name: 'Кюрасао' },
    { iso2: 'cx', dialCode: '+61', name: 'Рождественский остров' },
    { iso2: 'cy', dialCode: '+357', name: 'Кипр' },
    { iso2: 'cz', dialCode: '+420', name: 'Чехия' },
    { iso2: 'de', dialCode: '+49', name: 'Германия' },
    { iso2: 'dj', dialCode: '+253', name: 'Джибути' },
    { iso2: 'dk', dialCode: '+45', name: 'Дания' },
    { iso2: 'dm', dialCode: '+1', name: 'Доминика' },
    { iso2: 'do', dialCode: '+1', name: 'Доминиканская Республика' },
    { iso2: 'dz', dialCode: '+213', name: 'Алжир' },
    { iso2: 'ec', dialCode: '+593', name: 'Эквадор' },
    { iso2: 'ee', dialCode: '+372', name: 'Эстония' },
    { iso2: 'eg', dialCode: '+20', name: 'Египет' },
    { iso2: 'eh', dialCode: '+212', name: 'Западная Сахара' },
    { iso2: 'er', dialCode: '+291', name: 'Эритрея' },
    { iso2: 'es', dialCode: '+34', name: 'Испания' },
    { iso2: 'et', dialCode: '+251', name: 'Эфиопия' },
    { iso2: 'fi', dialCode: '+358', name: 'Финляндия' },
    { iso2: 'fj', dialCode: '+679', name: 'Фиджи' },
    { iso2: 'fk', dialCode: '+500', name: 'Фолклендские острова' },
    { iso2: 'fm', dialCode: '+691', name: 'Микронезия' },
    { iso2: 'fo', dialCode: '+298', name: 'Фарерские острова' },
    { iso2: 'fr', dialCode: '+33', name: 'Франция' },
    { iso2: 'ga', dialCode: '+241', name: 'Габон' },
    { iso2: 'gb', dialCode: '+44', name: 'Великобритания' },
    { iso2: 'gd', dialCode: '+1', name: 'Гренада' },
    { iso2: 'ge', dialCode: '+995', name: 'Грузия' },
    { iso2: 'gf', dialCode: '+594', name: 'Французская Гвиана' },
    { iso2: 'gg', dialCode: '+44', name: 'Гернси' },
    { iso2: 'gh', dialCode: '+233', name: 'Гана' },
    { iso2: 'gi', dialCode: '+350', name: 'Гибралтар' },
    { iso2: 'gl', dialCode: '+299', name: 'Гренландия' },
    { iso2: 'gm', dialCode: '+220', name: 'Гамбия' },
    { iso2: 'gn', dialCode: '+224', name: 'Гвинея' },
    { iso2: 'gp', dialCode: '+590', name: 'Гваделупа' },
    { iso2: 'gq', dialCode: '+240', name: 'Экваториальная Гвинея' },
    { iso2: 'gr', dialCode: '+30', name: 'Греция' },
    { iso2: 'gs', dialCode: '+500', name: 'Южная Георгия и Южные Сандвичевы острова' },
    { iso2: 'gt', dialCode: '+502', name: 'Гватемала' },
    { iso2: 'gu', dialCode: '+1', name: 'Гуам' },
    { iso2: 'gw', dialCode: '+245', name: 'Гвинея-Бисау' },
    { iso2: 'gy', dialCode: '+592', name: 'Гайана' },
    { iso2: 'hk', dialCode: '+852', name: 'Гонконг' },
    { iso2: 'hm', dialCode: '+61', name: 'Остров Херд и острова Макдональд' },
    { iso2: 'hn', dialCode: '+504', name: 'Гондурас' },
    { iso2: 'hr', dialCode: '+385', name: 'Хорватия' },
    { iso2: 'ht', dialCode: '+509', name: 'Гаити' },
    { iso2: 'hu', dialCode: '+36', name: 'Венгрия' },
    { iso2: 'id', dialCode: '+62', name: 'Индонезия' },
    { iso2: 'ie', dialCode: '+353', name: 'Ирландия' },
    { iso2: 'il', dialCode: '+972', name: 'Израиль' },
    { iso2: 'im', dialCode: '+44', name: 'Остров Мэн' },
    { iso2: 'in', dialCode: '+91', name: 'Индия' },
    { iso2: 'io', dialCode: '+246', name: 'Британская территория в Индийском океане' },
    { iso2: 'iq', dialCode: '+964', name: 'Ирак' },
    { iso2: 'ir', dialCode: '+98', name: 'Иран' },
    { iso2: 'is', dialCode: '+354', name: 'Исландия' },
    { iso2: 'it', dialCode: '+39', name: 'Италия' },
    { iso2: 'je', dialCode: '+44', name: 'Джерси' },
    { iso2: 'jm', dialCode: '+1', name: 'Ямайка' },
    { iso2: 'jo', dialCode: '+962', name: 'Иордания' },
    { iso2: 'jp', dialCode: '+81', name: 'Япония' },
    { iso2: 'ke', dialCode: '+254', name: 'Кения' },
    { iso2: 'kg', dialCode: '+996', name: 'Киргизия' },
    { iso2: 'kh', dialCode: '+855', name: 'Камбоджа' },
    { iso2: 'ki', dialCode: '+686', name: 'Кирибати' },
    { iso2: 'km', dialCode: '+269', name: 'Коморские острова' },
    { iso2: 'kn', dialCode: '+1', name: 'Сент-Китс и Невис' },
    { iso2: 'kp', dialCode: '+850', name: 'Северная Корея' },
    { iso2: 'kr', dialCode: '+82', name: 'Южная Корея' },
    { iso2: 'kw', dialCode: '+965', name: 'Кувейт' },
    { iso2: 'ky', dialCode: '+1', name: 'Острова Кайман' },
    { iso2: 'kz', dialCode: '+7', name: 'Казахстан' },
    { iso2: 'la', dialCode: '+856', name: 'Лаос' },
    { iso2: 'lb', dialCode: '+961', name: 'Ливан' },
    { iso2: 'lc', dialCode: '+1', name: 'Сент-Люсия' },
    { iso2: 'li', dialCode: '+423', name: 'Лихтенштейн' },
    { iso2: 'lk', dialCode: '+94', name: 'Шри-Ланка' },
    { iso2: 'lr', dialCode: '+231', name: 'Либерия' },
    { iso2: 'ls', dialCode: '+266', name: 'Лесото' },
    { iso2: 'lt', dialCode: '+370', name: 'Литва' },
    { iso2: 'lu', dialCode: '+352', name: 'Люксембург' },
    { iso2: 'lv', dialCode: '+371', name: 'Латвия' },
    { iso2: 'ly', dialCode: '+218', name: 'Ливия' },
    { iso2: 'ma', dialCode: '+212', name: 'Марокко' },
    { iso2: 'mc', dialCode: '+377', name: 'Монако' },
    { iso2: 'md', dialCode: '+373', name: 'Молдова' },
    { iso2: 'me', dialCode: '+382', name: 'Черногория' },
    { iso2: 'mf', dialCode: '+590', name: 'Сен-Мартен' },
    { iso2: 'mg', dialCode: '+261', name: 'Мадагаскар' },
    { iso2: 'mh', dialCode: '+692', name: 'Маршаллевы острова' },
    { iso2: 'mk', dialCode: '+389', name: 'Северная Македония' },
    { iso2: 'ml', dialCode: '+223', name: 'Мали' },
    { iso2: 'mm', dialCode: '+95', name: 'Мьянма' },
    { iso2: 'mn', dialCode: '+976', name: 'Монголия' },
    { iso2: 'mo', dialCode: '+853', name: 'Макао' },
    { iso2: 'mp', dialCode: '+1', name: 'Северные Марианские острова' },
    { iso2: 'mq', dialCode: '+596', name: 'Мартиника' },
    { iso2: 'mr', dialCode: '+222', name: 'Мавритания' },
    { iso2: 'ms', dialCode: '+1', name: 'Монтсеррат' },
    { iso2: 'mt', dialCode: '+356', name: 'Мальта' },
    { iso2: 'mu', dialCode: '+230', name: 'Маврикий' },
    { iso2: 'mv', dialCode: '+960', name: 'Мальдивы' },
    { iso2: 'mw', dialCode: '+265', name: 'Малави' },
    { iso2: 'mx', dialCode: '+52', name: 'Мексика' },
    { iso2: 'my', dialCode: '+60', name: 'Малайзия' },
    { iso2: 'mz', dialCode: '+258', name: 'Мозамбик' },
    { iso2: 'na', dialCode: '+264', name: 'Намибия' },
    { iso2: 'nc', dialCode: '+687', name: 'Новая Каледония' },
    { iso2: 'ne', dialCode: '+227', name: 'Нигер' },
    { iso2: 'nf', dialCode: '+672', name: 'Остров Норфолк' },
    { iso2: 'ng', dialCode: '+234', name: 'Нигерия' },
    { iso2: 'ni', dialCode: '+505', name: 'Никарагуа' },
    { iso2: 'nl', dialCode: '+31', name: 'Нидерланды' },
    { iso2: 'no', dialCode: '+47', name: 'Норвегия' },
    { iso2: 'np', dialCode: '+977', name: 'Непал' },
    { iso2: 'nr', dialCode: '+674', name: 'Науру' },
    { iso2: 'nu', dialCode: '+683', name: 'Ниуэ' },
    { iso2: 'nz', dialCode: '+64', name: 'Новая Зеландия' },
    { iso2: 'om', dialCode: '+968', name: 'Оман' },
    { iso2: 'pa', dialCode: '+507', name: 'Панама' },
    { iso2: 'pe', dialCode: '+51', name: 'Перу' },
    { iso2: 'pf', dialCode: '+689', name: 'Французская Полинезия' },
    { iso2: 'pg', dialCode: '+675', name: 'Папуа - Новая Гвинея' },
    { iso2: 'ph', dialCode: '+63', name: 'Филиппины' },
    { iso2: 'pk', dialCode: '+92', name: 'Пакистан' },
    { iso2: 'pl', dialCode: '+48', name: 'Польша' },
    { iso2: 'pm', dialCode: '+508', name: 'Сен-Пьер и Микелон' },
    { iso2: 'pn', dialCode: '+64', name: 'Острова Питкэрн' },
    { iso2: 'pr', dialCode: '+1', name: 'Пуэрто-Рико' },
    { iso2: 'ps', dialCode: '+970', name: 'Палестина' },
    { iso2: 'pt', dialCode: '+351', name: 'Португалия' },
    { iso2: 'pw', dialCode: '+680', name: 'Палау' },
    { iso2: 'py', dialCode: '+595', name: 'Парагвай' },
    { iso2: 'qa', dialCode: '+974', name: 'Катар' },
    { iso2: 're', dialCode: '+262', name: 'Реюньон' },
    { iso2: 'ro', dialCode: '+40', name: 'Румыния' },
    { iso2: 'rs', dialCode: '+381', name: 'Сербия' },
    { iso2: 'ru', dialCode: '+7', name: 'Россия' },
    { iso2: 'rw', dialCode: '+250', name: 'Руанда' },
    { iso2: 'sa', dialCode: '+966', name: 'Саудовская Аравия' },
    { iso2: 'sb', dialCode: '+677', name: 'Соломоновы острова' },
    { iso2: 'sc', dialCode: '+248', name: 'Сейшельские острова' },
    { iso2: 'sd', dialCode: '+249', name: 'Судан' },
    { iso2: 'se', dialCode: '+46', name: 'Швеция' },
    { iso2: 'sg', dialCode: '+65', name: 'Сингапур' },
    { iso2: 'sh', dialCode: '+290', name: 'Остров Святой Елены' },
    { iso2: 'si', dialCode: '+386', name: 'Словения' },
    { iso2: 'sj', dialCode: '+47', name: 'Шпицберген и Ян-Майен' },
    { iso2: 'sk', dialCode: '+421', name: 'Словакия' },
    { iso2: 'sl', dialCode: '+232', name: 'Сьерра-Леоне' },
    { iso2: 'sm', dialCode: '+378', name: 'Сан-Марино' },
    { iso2: 'sn', dialCode: '+221', name: 'Сенегал' },
    { iso2: 'so', dialCode: '+252', name: 'Сомали' },
    { iso2: 'sr', dialCode: '+597', name: 'Суринам' },
    { iso2: 'ss', dialCode: '+211', name: 'Южный Судан' },
    { iso2: 'st', dialCode: '+239', name: 'Сан-Томе и Принсипи' },
    { iso2: 'sv', dialCode: '+503', name: 'Сальвадор' },
    { iso2: 'sx', dialCode: '+1', name: 'Синт-Мартен' },
    { iso2: 'sy', dialCode: '+963', name: 'Сирия' },
    { iso2: 'sz', dialCode: '+268', name: 'Эсватини' },
    { iso2: 'tc', dialCode: '+1', name: 'Острова Теркс и Кайкос' },
    { iso2: 'td', dialCode: '+235', name: 'Чад' },
    { iso2: 'tf', dialCode: '+262', name: 'Французские Южные и Антарктические земли' },
    { iso2: 'tg', dialCode: '+228', name: 'Того' },
    { iso2: 'th', dialCode: '+66', name: 'Таиланд' },
    { iso2: 'tj', dialCode: '+992', name: 'Таджикистан' },
    { iso2: 'tk', dialCode: '+690', name: 'Токелау' },
    { iso2: 'tl', dialCode: '+670', name: 'Восточный Тимор' },
    { iso2: 'tm', dialCode: '+993', name: 'Туркменистан' },
    { iso2: 'tn', dialCode: '+216', name: 'Тунис' },
    { iso2: 'to', dialCode: '+676', name: 'Тонга' },
    { iso2: 'tr', dialCode: '+90', name: 'Турция' },
    { iso2: 'tt', dialCode: '+1', name: 'Тринидад и Тобаго' },
    { iso2: 'tv', dialCode: '+688', name: 'Тувалу' },
    { iso2: 'tw', dialCode: '+886', name: 'Тайвань' },
    { iso2: 'tz', dialCode: '+255', name: 'Танзания' },
    { iso2: 'ua', dialCode: '+380', name: 'Украина' },
    { iso2: 'ug', dialCode: '+256', name: 'Уганда' },
    { iso2: 'us', dialCode: '+1', name: 'США' },
    { iso2: 'uy', dialCode: '+598', name: 'Уругвай' },
    { iso2: 'uz', dialCode: '+998', name: 'Узбекистан' },
    { iso2: 'va', dialCode: '+379', name: 'Ватикан' },
    { iso2: 've', dialCode: '+58', name: 'Венесуэла' },
    { iso2: 'vg', dialCode: '+1', name: 'Британские Виргинские острова' },
    { iso2: 'vi', dialCode: '+1', name: 'Американские Виргинские острова' },
    { iso2: 'vn', dialCode: '+84', name: 'Вьетнам' },
    { iso2: 'vu', dialCode: '+678', name: 'Вануату' },
    { iso2: 'wf', dialCode: '+681', name: 'Уоллис и Футуна' },
    { iso2: 'ws', dialCode: '+685', name: 'Самоа' },
    { iso2: 'xk', dialCode: '+383', name: 'Косово' },
    { iso2: 'ye', dialCode: '+967', name: 'Йемен' },
    { iso2: 'yt', dialCode: '+262', name: 'Майотта' },
    { iso2: 'za', dialCode: '+27', name: 'Южно-Африканская Республика' },
    { iso2: 'zm', dialCode: '+260', name: 'Замбия' },
    { iso2: 'zw', dialCode: '+263', name: 'Зимбабве' }
  ];

  $: isPhoneValid = phoneNumber.trim().length > 0 && !phoneError;
  $: isCodeValid = verificationCode.length === 4 && !codeError;
  $: canConnect = step === 2 && isCodeValid;
  $: filteredCountries = countries.filter(c => 
    c.name.toLowerCase().includes(searchQuery.toLowerCase()) || 
    c.dialCode.includes(searchQuery)
  );

  const handleSendCode = async () => {
    phoneError = '';
    
    if (!phoneNumber.trim()) {
      phoneError = 'Введите номер телефона';
      return;
    }

    isLoadingSendCode = true;

    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:SendPhoneVerificationCode", {
        phone: phoneNumber,
        country: country
      }, (response) => {
        isLoadingSendCode = false;
        if (response?.success) {
          step = 2;
        } else {
          phoneError = response?.message || 'Ошибка при отправке кода';
        }
      });
    }
  };

  const handlePhoneInput = (event) => {
    phoneNumber = event.target.value;
    phoneError = '';
  };

  const handleCodeInput = (event) => {
    let value = event.target.value.replace(/[^0-9]/g, '');
    if (value.length > 4) value = value.slice(0, 4);
    verificationCode = value;
    codeError = '';
  };

  const handleConnect = () => {
    if (verificationCode.length !== 4) {
      codeError = 'Введите 4-значный код';
      return;
    }

    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:ConfirmPhoneNumber", {
        phone: phoneNumber,
        code: verificationCode,
        country: country
      });
    }
    closeModal();
  };

  const closeModal = () => {
    dispatch('close');
  };

  const handleCountrySelect = (countryCode) => {
    country = countryCode.toUpperCase();
    showCountrySelect = false;
    searchQuery = '';
  };

  const toggleCountrySelect = () => {
    showCountrySelect = !showCountrySelect;
    searchQuery = '';
  };

  const handleSearchInput = (event) => {
    searchQuery = event.target.value;
  };
</script>

<div data-v-322e10d6 data-v-c3f491b2 data-v-5929cd6e-s class="modal-wrapper">
  <div data-v-322e10d6 class="modal">
    <div data-v-322e10d6 class="title">Привязать номер телефона</div>
    <div data-v-322e10d6 class="close-btn" on:click={closeModal}>
      <SVGComponent path="icons/F4/Settings/cross.svg" />
    </div>

    <div data-v-364bd745 data-v-322e10d6 class="enablePhone">
      <p data-v-364bd745>
        На введенный номер телефона будет отправлен код для привязки.
      </p>

      <!-- Step 1: Enter Phone Number -->
      <div 
        data-v-364bd745 
        class="mobile-container"
        class:disable={step === 2}
      >
        <div data-v-364bd745 class="phone-data">
          <div data-v-b88dc7cd data-v-364bd745 class="m-phone-number-input phone-input --row">
            <!-- Country Selector -->
            <div data-v-1e2814b9 data-v-b88dc7cd class="m-country-selector">
              <button
                data-v-1e2814b9
                id="country-selector-flag-button"
                class="m-country-selector__country-flag maz-text-xl"
                tabindex="-1"
                type="button"
                on:click={toggleCountrySelect}
              >
                <img
                  data-v-364bd745
                  src="https://cdn.majestic-files.com/public/master/static/icons/flags/{country.toLowerCase()}.svg"
                  alt="{country}.svg"
                />
                <span data-v-364bd745 class="country-code">{country}</span>
              </button>

              <!-- Country Select Dropdown -->
              <div
                data-v-cc87f1d6
                data-v-1e2814b9
                class="m-select m-country-selector__select --md"
                class:--is-open={showCountrySelect}
                style="width: 9rem;"
              >
                <div data-v-e9affc48 data-v-cc87f1d6 class="m-input --block m-select-input --primary --md" class:--is-focused={showCountrySelect}>
                  <div data-v-e9affc48 class="m-input-wrapper --default-border maz-rounded" class:maz-border-primary={showCountrySelect}>
                    <div data-v-e9affc48 class="m-input-wrapper-input">
                      <input
                        data-v-e9affc48
                        id="country-selector-input"
                        type="text"
                        maxlength="15"
                        
                        inputmode="text"
                        class="m-input-input"
                        readonly
                      />
                    </div>
                    <div data-v-e9affc48 class="m-input-wrapper-right">
                      <button
                        data-v-cc87f1d6
                        tabindex="-1"
                        type="button"
                        class="m-select-input__toggle-button maz-custom"
                        aria-label={showCountrySelect ? 'collapse list of options' : 'expand list of options'}
                        on:click={toggleCountrySelect}
                      >
                        <svg
                          data-v-cc87f1d6
                          xmlns="http://www.w3.org/2000/svg"
                          width="1em"
                          height="1em"
                          fill="none"
                          viewBox="0 0 24 24"
                          class="m-select-chevron maz-text-xl"
                        >
                          <path
                            stroke="currentColor"
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="1.5"
                            d="m19.5 8.25-7.5 7.5-7.5-7.5"
                          />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>

                {#if showCountrySelect}
                  <div data-v-cc87f1d6 class="m-select-list --left --bottom" style="max-height: 240px; max-width: 250px;">
                    <div data-v-e9affc48 data-v-cc87f1d6 class="m-input m-select-list__search-input maz-flex-none --primary --sm">
                      <div data-v-e9affc48 class="m-input-wrapper --default-border maz-rounded">
                        <div data-v-e9affc48 class="m-input-wrapper-left">
                          <svg
                            data-v-e9affc48
                            xmlns="http://www.w3.org/2000/svg"
                            width="1em"
                            height="1em"
                            fill="none"
                            viewBox="0 0 24 24"
                            class="maz-text-xl maz-text-muted"
                          >
                            <path
                              stroke="currentColor"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                              stroke-width="1.5"
                              d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607"
                            />
                          </svg>
                        </div>
                        <div data-v-e9affc48 class="m-input-wrapper-input">
                          <input
                            data-v-e9affc48
                            id="country-search-input"
                            type="text"
                            name="search"
                            autocomplete="off"
                            inputmode="text"
                            placeholder="Поиск"
                            aria-label="Поиск"
                            class="m-input-input"
                            value={searchQuery}
                            on:input={handleSearchInput}
                          />
                        </div>
                      </div>
                    </div>

                     <div data-v-cc87f1d6 class="m-select-list__scroll-wrapper" tabindex="-1">
                      {#each filteredCountries as c (c.iso2)}
                        <button
                          data-v-cc87f1d6
                          tabindex="-1"
                          type="button"
                          class="m-select-list-item maz-custom maz-flex-none"
                          style="height: 38px;"
                          on:click={() => handleCountrySelect(c.iso2)}
                        >
                          <div data-v-1e2814b9 class="m-country-selector__select__item maz-flex maz-items-center maz-gap-1 maz-truncate">
                            <span data-v-1e2814b9 class="maz-text-lg">
                              <img
                                data-v-364bd745
                                src="https://cdn.majestic-files.com/public/master/static/icons/flags/{c.iso2}.svg"
                                alt="{c.iso2}.svg"
                              />
                              <span data-v-364bd745>{c.dialCode}</span>
                            </span>
                            <span data-v-1e2814b9 class="maz-flex-1 maz-truncate">{c.name}</span>
                          </div>
                        </button>
                      {/each}
                    </div>
                  </div>
                {/if}
              </div>
            </div>

            <!-- Phone Input -->
            <div 
              data-v-e9affc48 
              data-v-f3c379ea 
              data-v-b88dc7cd 
              class="m-input --block m-phone-input --border-radius --primary --md"
              class:--error={step === 1 && !!phoneError}
            >
              <div data-v-e9affc48 class="m-input-wrapper --default-border maz-rounded">
                <div data-v-e9affc48 class="m-input-wrapper-input">
                  <input
                    data-v-e9affc48
                    id="MazPhoneNumberInput"
                    type="tel"
                    maxlength="15"
                    inputmode="tel"
                    placeholder="000–000–0000"
                    aria-label="000–000–0000"
                    class="m-input-input"
                    value={phoneNumber}
                    on:input={handlePhoneInput}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Send Code Button -->
        <div 
          data-v-364bd745 
          class="request-code-button"
          class:disable={!isPhoneValid || isLoadingSendCode}
        >
          <button
            data-v-364bd745
            on:click={handleSendCode}
            disabled={!isPhoneValid || isLoadingSendCode}
            type="button"
            class="send-button"
          >
            <div data-v-364bd745 class="content">
              <p data-v-364bd745>
                {isLoadingSendCode ? 'Отправка...' : 'Отправить код'}
              </p>
            </div>
          </button>
        </div>
      </div>

      <!-- Step 2: Enter Verification Code -->
      <div 
        data-v-364bd745 
        class="mobile-input"
        class:disable={step === 1}
      >
        <p data-v-364bd745>Введите полученный код</p>
        <div data-v-364bd745 class="input-wrapper">
          <input
            data-v-364bd745
            type="text"
            autocomplete="off"
            maxlength="4"
            inputmode="numeric"
            placeholder="Код"
            value={verificationCode}
            on:input={handleCodeInput}
          />
        </div>
      </div>

      <!-- Control Buttons -->
      <div data-v-364bd745 class="control-buttons">
        <button
          data-v-364bd745
          class="button connect {canConnect ? '' : 'disabled'}"
          disabled={!canConnect}
          on:click={handleConnect}
          type="button"
        >
          <div data-v-364bd745 class="button-content">
            <span data-v-364bd745>Подключить</span>
          </div>
        </button>
        <button
          data-v-364bd745
          class="button cancel"
          on:click={closeModal}
          type="button"
        >
          <div data-v-364bd745 class="button-content">
            <span data-v-364bd745>Отменить</span>
          </div>
        </button>
      </div>
    </div>
  </div>
</div>

<style>
  .send-button {
    background: none;
    border: none;
    color: inherit;
    cursor: pointer;
    width: 100%;
  }

  .send-button:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }

  .send-button {
    background: none;
    border: none;
    color: inherit;
    cursor: pointer;
    width: 100%;
  }

  .send-button:disabled {
    cursor: not-allowed;
    opacity: 0.5;
  }

  :global(.m-select.--is-open .m-select-list) {
    display: block;
  }

  :global(.m-select-list) {
    display: none;
  }

  :global(.m-country-selector__country-flag) {
    cursor: pointer;
    padding: 8px;
    border-radius: 4px;
  }

  :global(.m-country-selector__country-flag:hover) {
    background: rgba(255, 255, 255, 0.1);
  }
</style>