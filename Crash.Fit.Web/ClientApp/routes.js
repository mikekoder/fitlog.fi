import Layout from './views/layout.vue'
import Home from './views/home/home.vue'

import Meals from './views/meals/meals.vue'
import MealRhythm from './views/meal-rhythm/meal-rhythm.vue'
import MealDetails from './views/meals/meal-details.vue'
import Foods from './views/foods/foods.vue'
import FoodDetails from './views/foods/food-details.vue'
import Recipes from './views/recipes/recipes.vue'
import RecipeDetails from './views/recipes/recipe-details.vue'
import Nutrients from './views/nutrients/nutrients.vue'
import NutritionGoals from './views/nutrition-goals/nutrition-goals.vue'
import NutritionGoalDetails from './views/nutrition-goals/nutrition-goal-details.vue'

import Workouts from './views/workouts/workouts.vue'
import WorkoutDetails from './views/workouts/workout-details.vue'
import Exercises from './views/exercises/exercises.vue'
import ExerciseDetails from './views/exercises/exercise-details.vue'
import Routines from './views/routines/routines.vue'
import RoutineDetails from './views/routines/routine-details.vue'
import RepCalculator from './views/rep-calculator/rep-calculator.vue'
import TrainingGoals from './views/training-goals/training-goals.vue'
import TrainingGoalDetails from './views/training-goals/training-goal-details.vue'
import EnergyExpenditures from './views/energy-expenditures/energy-expenditures.vue'
import ActivityLevels from './views/activity-levels/activity-levels.vue'

import Profile from './views/profile/profile.vue'
import Login from './views/login.vue'
import LoginSuccess from './views/login-success.vue'
import Register from './views/register.vue'

import Measurements from './views/measurements/measurements.vue'

import Feedback from './views/feedback/feedback.vue'
import FeedbackDetails from './views/feedback/feedback-details.vue'

import PrivacySecurity from './views/privacy-security.vue'

import NotFound from './views/notfound.vue'

export default [
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
                path: '/ravintotavoitteet/:id',
                name: 'nutrition-goal-details',
                component: NutritionGoalDetails
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
                path: '/aktiivisuustasot',
                name: 'activity-levels',
                component: ActivityLevels
            },
            {
                path: '/kulutukset',
                name: 'energy-expenditures',
                component: EnergyExpenditures
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
                path: '/treenitavoitteet',
                name: 'training-goals',
                component: TrainingGoals
            },
            {
                path: '/treenitavoitteet/:id',
                name: 'training-goal-details',
                component: TrainingGoalDetails
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
                meta: { type: 'Bug' }
            },
            {
                path: '/bugit/:id',
                name: 'bug-details',
                component: FeedbackDetails,
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
            },
            {
                path: '/tietosuoja',
                name: 'privacy-security',
                component: PrivacySecurity,
                meta: { anon: true }
            },
        ]
    },
    {
        path: '/kirjaudu/:client?',
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
