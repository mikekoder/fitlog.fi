export default {
  lang: 'fi',
  label: {
    clear: 'Tyhjennä',
    ok: 'OK',
    cancel: 'Peruuta',
    close: 'Sulje',
    set: 'Aseta',
    select: 'Valitse',
    reset: 'Nollaa',
    remove: 'Poista',
    update: 'Päivitä',
    create: 'Luo',
    search: 'Etsi',
    filter: 'Suodatin',
    refresh: 'Päivitä'
  },
  date: {
    days: 'Sunnuntai_Maanantai_Tiistai_Keskiviikko_Torstai_Perjantai_Lauantai'.split('_'),
    daysShort: 'Su_Ma_Ti_Ke_To_Pe_La'.split('_'),
    months: 'Tammikuu_Helmikuu_Maaliskuu_Huhtikuu_Toukokuu_Kesäkuu_Heinäkuu_Elokuu_Syyskuu_Lokakuu_Marraskuu_Joulukuu'.split('_'),
    monthsShort: 'Tam_Hel_Maa_Huh_Tou_Kes_Hei_Elo_Syy_Lok_Mar_Jou'.split('_'),
    firstDayOfWeek: 1, // 0-6, 0 - Sunday, 1 Monday, ...
    format24h: true
  },
  pullToRefresh: {
    pull: 'Vedä alas päivittääksesi',
    release: 'Vapauta päivittääksesi',
    refresh: 'Päivitetään...'
  },
  table: {
    noData: 'Ei tietoja',
    noResults: 'Ei tuloksia',
    loading: 'Ladataan...',
    selectedRows: function (rows) {
      return rows === 1
        ? '1 valittu rivi.'
        : (rows === 0 ? 'Ei valittuja rivejä' : rows  + ' valittua riviä.')
    },
    rowsPerPage: 'Rivejä sivulla:',
    allRows: 'Kaikki',
    pagination: function (start, end, total) {
      return start + '-' + end + ' / ' + total
    },
    columns: 'Sarakkeet'
  },
  editor: {
    url: 'URL',
    bold: 'Lihavoitu',
    italic: 'Kursivoitu',
    strikethrough: 'Yliviivattu',
    underline: 'Alleviivattu',
    unorderedList: 'Luettelomerkit',
    orderedList: 'Numeroitu',
    subscript: 'Alaindeksi',
    superscript: 'Yläindeksi',
    hyperlink: 'Hyperlinkki',
    toggleFullscreen: 'Koko näyttö',
    quote: 'Lainaus',
    left: 'Tasaa vasemmalle',
    center: 'Keskitä',
    right: 'Tasaa oikealle',
    justify: 'Tasaa',
    print: 'Tulosta',
    outdent: 'Pienennä sisennystä',
    indent: 'Suurenna sisennystä',
    removeFormat: 'Poista muotoilu',
    formatting: 'Muotoilu',
    fontSize: 'Fonttikoko',
    align: 'Tasaus',
    hr: 'Lisää vaakaviiva',
    undo: 'Kumoa',
    redo: 'Palauta',
    header1: 'Otsikko 1',
    header2: 'Otsikko 2',
    header3: 'Otsikko 3',
    header4: 'Otsikko 4',
    header5: 'Otsikko 5',
    header6: 'Otsikko 6',
    paragraph: 'Kappale',
    code: 'Koodi',
    size1: 'Erittäin pieni',
    size2: 'Pieni',
    size3: 'Normaali',
    size4: 'Keskisuuri',
    size5: 'Suuri',
    size6: 'Erittäin suuri',
    size7: 'Maksimi',
    defaultFont: 'Oletusfontti'
  },
  tree: {
    noNodes: 'Ei solmuja',
    noResults: 'Ei tuloksia'
  }
}
