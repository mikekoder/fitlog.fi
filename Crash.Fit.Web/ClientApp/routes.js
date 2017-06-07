import Layout from './views/layout'
import Home from './views/home'
import Meals from './views/meals/meals'
import MealDetails from './views/meals/meal-details'
import Foods from './views/foods/foods'
import FoodDetails from './views/foods/food-details'
import Recipes from './views/recipes/recipes'
import RecipeDetails from './views/recipes/recipe-details'
import Nutrients from './views/nutrients/nutrients'
import NutritionTargets from './views/nutrition-targets/nutrition-targets'

import Workouts from './views/workouts/workouts'
import WorkoutDetails from './views/workouts/workout-details'
import Exercises from './views/exercises/exercises'
import ExerciseDetails from './views/exercises/exercise-details'
import Routines from './views/routines/routines'
import RoutineDetails from './views/routines/routine-details'
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
                path: '/ateriat',
                name: 'meals',
                component: Meals
            },
            {
                path: '/ateriat/:id/:action?',
                name: 'meal-details',
                component: MealDetails
            },
            {
                path: '/ruoka-aineet',
                name: 'foods',
                component: Foods
            },
            {
                path: '/ruoka-aineet/:id',
                name: 'food-details',
                component: FoodDetails
            },
            {
                path: '/reseptit',
                name: 'recipes',
                component: Recipes
            },
            {
                path: '/reseptit/:id',
                name: 'recipe-details',
                component: RecipeDetails
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
                path: '/treenit',
                name: 'workouts',
                component: Workouts
            },
            {
                path: '/treenit/:id',
                name: 'workout-details',
                component: WorkoutDetails
            },
            {
                path: '/liikkeet',
                name: 'exercises',
                component: Exercises
            },
            {
                path: '/liikkeet/:id?',
                name: 'exercise-details',
                component: ExerciseDetails
            },
            {
                path: '/ohjelmat',
                name: 'routines',
                component: Routines
            },
            {
                path: '/ohjelmat/:id',
                name: 'routine-details',
                component: RoutineDetails
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
        name: 'login-success',
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
