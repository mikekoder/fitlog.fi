﻿import Layout from './views/layout'
import Home from './views/home/home'

import Meals from './views/meals/meals'
import MealRhythm from './views/meal-rhythm/meal-rhythm'
import MealDetails from './views/meals/meal-details'
import Foods from './views/foods/foods'
import FoodDetails from './views/foods/food-details'
import Recipes from './views/recipes/recipes'
import RecipeDetails from './views/recipes/recipe-details'
import Nutrients from './views/nutrients/nutrients'
import NutritionGoals from './views/nutrition-goals/nutrition-goals'

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
import Feedback from './views/feedback/feedback'
import FeedbackDetails from './views/feedback/feedback-details'

const main = [
    {
        path: '/',
        component: Layout,
        children:[
            {
                path: '',
                name: 'home',
                component: Home,
                meta: { anon: true }
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
                path: '/ateriarytmi',
                name: 'mealrhythm',
                component: MealRhythm
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
                name: 'nutrition-goals',
                component: NutritionGoals
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
            {
                path: '/profiili',
                name: 'profile',
                component: Profile
            },
            {
                path: '/bugit',
                name: 'bugs',
                component: Feedback,
                params: { foo: 'bar'},
                meta: { type: 'Bug' }
            },
            {
                path: '/bugit/:id',
                name: 'bug-details',
                component: FeedbackDetails,
                params: { foo: 'bar' },
                meta: { type: 'Bug' }
            },
            {
                path: '/kehitysideat',
                name: 'improvements',
                component: Feedback,
                meta: { type: 'Improvement' }
            },
            {
                path: '/kehitysideat/:id',
                name: 'improvement-details',
                component: FeedbackDetails,
                meta: { type: 'Improvement' }
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
        path: '/login-success/:client/:refreshToken/:accessToken',
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
