const fi = {

    add: 'Lisää',
    all: 'Kaikki',
    amount: 'Määrä',
    average: 'Keskiarvo',
    cancel: 'Peruuta',
    chooseDateInterval: 'Valitse aikaväli',
    copy: 'Kopioi',
    createNew: 'Luo uusi',
    currentMonth: 'Kuluva kuukausi',
    currentWeek: 'Kuluva viikko',
    day: 'vrk',
    days: 'pv',
    'default': 'Oletus',
    'delete': 'Poista',
    details: 'Yksityiskohdat',
    dob: 'Syntymäaika',
    estimate: 'Arvio',
    food: 'Ruoka-aine',
    foods: 'Ruoka-aineet',
    gender: 'Sukupuoli',
    height: 'Pituus',
    hide: 'Piilota',
    name: 'Nimi',
    nutrients: 'Ravintoaineet',
    OK: 'OK',
    portion: 'Annos',
    portions: 'Annokset',
    reps: 'Toistot',
    save: 'Tallenna',
    show: 'Näytä',
    summary: 'Yhteenveto',
    time: 'Aika',
    today: 'Tänään',
    value: 'Arvo',
    visibility: 'Näkyvyys',
    weight: 'Paino',
    weights: 'Painot',
    
    monday: 'Maanantai',
    mondayShort: 'M',
    tuesday: 'Tiistai',
    tuesdayShort: 'T',
    wednesday: 'Keskiviikko',
    wednesdayShort: 'K',
    thursday: 'Torstai',
    thursdayShort: 'T',
    friday: 'Perjantai',
    fridayShort: 'P',
    saturday: 'Lauantai',
    saturdayShort: 'L',
    sunday: 'Sunnuntai',
    sundayShort: 'S',
    exerciseDay: 'Treeni',
    exerciseDayShort: 'T',
    restDay: 'Lepo',
    restDayShort: 'L',

    menu: {
        login: 'Kirjaudu',
        register: 'Rekisteröidy',
        profile: 'Profiili',
        logout: 'Kirjaudu ulos',

        nutritionHeader: 'Ravinto',
        meals: 'Ateriat',
        foods: 'Ruoka-aineet',
        recipes: 'Reseptit',
        nutrients: 'Ravintoaineet',
        nutritionTargets: 'Tavoitteet',

        trainingHeader: 'Treenaus',
        workouts: 'Treenit',
        exercises: 'Liikkeet',
        routines: 'Ohjelmat',
        repCalculator: 'Sarjapainolaskuri',

        measurementsHeader: 'Mitat',
        measurements: 'Mitat',

        mealCalculator: 'Aterialaskuri'
    },

    profile: {
        title: 'Profiili',
        rmr: 'Lepoaineenvaihdunta',
        pal:'Aktiivisuuskerroin'
    },

   

    

    meals: {
        title: 'Ateriat',
        create: 'Lisää ateria',
        noMeals: 'Ei aterioita',
        columns: {
            time: 'Aika',
            energyDistribution: 'Energiajakauma'
        },
        tabs: {
            foods: 'Ruoka-aineet',
            nutrients: 'Ravintoaineet'
        },
        mealDetails: 'Aterian tiedot'
    },

    foods: {
        title: 'Ruoka-aineet',
        create: 'Lisää ruoka-aine',
        noFoods: 'Ei ruoka-aineita',
        columns: {
            name: 'Nimi',
            usageCount: 'Käyttökertoja',
            nutrientCount: 'Ravintoarvoja'
        },
        tabs: {
            nutrients: 'Ravintoarvot',
            portions: 'Annokset'
        },
        foodDetails: 'Ruoka-aineet tiedot'
    },
    recipes: {
        title: 'Reseptit',
        create: 'Lisää resepti',
        noRecipes: 'Ei reseptejä',
        columns: {
            name: 'Nimi',
            usageCount: 'Käyttökertoja'
        },
        recipeDetails: 'Reseption tiedot',
        rawWeight: 'Raakapaino',
        cookedWeight: 'Valmispaino',
        weightChange: 'Valmistushävikki/-lisä'
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
    nutritionTargets:{
        title: 'Tavoitteet',
        addTarget: 'Lisää tavoite',
        onlyDays: 'Vain päivinä'
    },

    workouts: {
        title: 'Treenit'
    },
    exercises: {
        title: 'Liikkeet',
        create: 'Lisää liike',
        noExercises: 'Ei liikkeitä',
        columns: {
            name: 'Nimi',
            sets: 'Sarjoja'
        }
    },
    routines: {
        title: 'Ohjelmat',
        noRoutines: 'Ei ohjelmia'
    },
    repCalculator: {
        title: 'Sarjapainolaskuri',
        calculate: 'Laske'
    },

    measurements: {
        title: 'Mitat',
        create: 'Lisää',
        noMeasurements: 'Ei mittoja',
        measure: 'Mitta'
    }
}

module.exports = {
    fi: fi
};