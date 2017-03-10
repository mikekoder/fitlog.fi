import Layout from './views/layout'
import Home from './views/home'
import Meals from './views/meals/meals'
import MealCalculator from './views/meals/meal-calculator'
import Foods from './views/foods/foods'
import Recipes from './views/recipes'
import Nutrients from './views/nutrients'
import Workouts from './views/workouts'
import Exercises from './views/exercises'
import Routines from './views/routines'
import Login from './views/login'
import LoginSuccess from './views/login-success'
import Register from './views/register'
import NotFound from './views/notfound'

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
                path: '/ruoka-aineet',
                name: 'foods',
                component: Foods
            },
            {
                path: '/reseptit',
                name: 'recipes',
                component: Recipes
            },
            {
                path: '/ravintoaineet',
                name: 'nutrients',
                component: Nutrients
            },
            {
                path: '/treenit',
                name: 'workouts',
                component: Workouts
            },
            {
                path: '/harjoitukset',
                name: 'exercises',
                component: Exercises
            },
            {
                path: '/ohjelmat',
                name: 'routines',
                component: Routines
            },
            {
                path: '/sarjapainolaskuri',
                name: 'rep-calculator',
                component: Routines,
                meta: { anon: true }
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
