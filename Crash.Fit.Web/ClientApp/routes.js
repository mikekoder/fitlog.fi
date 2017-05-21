import Layout from './views/layout'
import Home from './views/home'
import Meals from './views/meals/meals'
import MealCalculator from './views/meals/meal-calculator'
import Foods from './views/foods/foods'
import Recipes from './views/recipes/recipes'
import Nutrients from './views/nutrients/nutrients'
import NutritionTargets from './views/nutrition-targets/nutrition-targets'
import Workouts from './views/workouts/workouts'
import Exercises from './views/exercises/exercises'
import Routines from './views/routines/routines'
import Login from './views/login'
import LoginSuccess from './views/login-success'
import Register from './views/register'
import NotFound from './views/notfound'
import RepCalculator from './views/rep-calculator/rep-calculator'
import Measurements from './views/measurements/measurements'
import Profile from './views/profile/profile'

const main = [
    {
        path: '/',
        component: Layout,
        children:[
            {
                path: '',
                name: 'home',
                component: Home
            },
            {
                path: '/ateriat/:id?',
                name: 'meals',
                component: Meals
            },
            {
                path: '/aterialaskuri',
                name: 'meal-calculator',
                component: MealCalculator
            },
            {
                path: '/ruoka-aineet/:id?',
                name: 'foods',
                component: Foods
            },
            {
                path: '/reseptit/:id?',
                name: 'recipes',
                component: Recipes
            },
            {
                path: '/ravintoaineet',
                name: 'nutrients',
                component: Nutrients
            },
            {
                path: '/ravintotavoitteet',
                name: 'nutrition-targets',
                component: NutritionTargets
            },
            {
                path: '/treenit/:id?',
                name: 'workouts',
                component: Workouts
            },
            {
                path: '/liikkeet/:id?',
                name: 'exercises',
                component: Exercises
            },
            {
                path: '/ohjelmat/:id?',
                name: 'routines',
                component: Routines
            },
            {
                path: '/sarjapainolaskuri',
                name: 'rep-calculator',
                component: RepCalculator,
                meta: { anon: true }
            },
            {
                path: '/mitat',
                name: 'measurements',
                component: Measurements
            },
            ,
            {
                path: '/profiili',
                name: 'profile',
                component: Profile
            }
        ]
    },
    {
        path: '/kirjaudu',
        name: 'login',
        component: Login,
        meta: { anon: true }
    },
    {
        path: '/login-success',
        component: LoginSuccess,
        meta: { anon: true }
    },
    {
        path: '/luo-tunnus',
        name: 'register',
        component: Register,
        meta: { anon: true }
    },
    {
        path: '*',
        name: 'notfound',
        component: NotFound,
        meta: { anon: true }
    }
]

export default main
