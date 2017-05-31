const fi = {

    OK: 'OK',
    save: 'Tallenna',
    cancel: 'Peruuta',
    'delete': 'Poista',
    add: 'Lisää',

    name: 'Nimi',
    amount: 'Määrä',
    weight: 'Paino',
    portion: 'Annos',
    time: 'Aika',

    today: 'Tänään',
    currentWeek: 'Kuluva viikko',
    currentMonth: 'Kuluva kuukausi',
    days: 'pv',
    chooseDateInterval: 'Valitse aikaväli',

    meals: {
        title: 'Ateriat',
        create: 'Lisää ateria',
        columns: {
            time: 'Aika',
            energyDistribution: 'Energiajakauma'
        },
        noMeals: 'Ei aterioita'
    },
    foods: {
        title: 'Ruoka-aineet',
        create: 'Lisää ruoka-aine',
        columns: {
            name: 'Nimi',
            usageCount: 'Käyttökertoja',
            nutrientCount: 'Ravintoarvoja'
        }
    },
    recipes: {
        title: 'Reseptit',
        create: 'Lisää resepti',
        columns: {
            name: 'Nimi',
            usageCount: 'Käyttökertoja'
        }
    },
    nutrients: {
        title: 'Ravintoaineet',
        groups: {
            MACROCMP: 'Makrot',
            VITAM: 'Vitamiinit',
            MINERAL: 'Mineraalit',
            CARBOCMP: 'Hiilihydraatit',
            FAT:' Rasvat'
        },
        columns: {
            visibility: 'Näkyvyys',
            summary: 'Yhteenveto',
            details: 'Yksityiskohdat'
        },
        options: {
            'default': 'Oletus',
            show: 'Näytä',
            hide: 'Piilota'
        }
    },
    nutrientTargets:{
        title: 'Tavoitteet'
    },
    exercises: {
        title: 'Liikkeet',
        create: 'Lisää liike',
        columns: {
            name: 'Nimi',
            sets: 'Sarjoja'
        }
    }
}

module.exports = {
    fi: {}
};