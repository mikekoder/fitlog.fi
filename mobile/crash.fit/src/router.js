import Vue from 'vue'
import VueRouter from 'vue-router'
import Layout from './views/layout'
import Home from './views/home/home.vue'
import Login from './views/login.vue'
import Foods from './views/foods/foods.vue'
import FoodDetails from './views/foods/food-details.vue'
import Recipes from './views/recipes/recipes.vue'
import RecipeDetails from './views/recipes/recipe-details.vue'
import MealRhythm from './views/meal-rhythm/meal-rhythm.vue'
import Workouts from './views/workouts/workouts.vue'
import WorkoutDetails from './views/workouts/workout-details.vue'
import WorkoutDetailsGrouped from './views/workouts/workout-details-grouped.vue'
import Exercises from './views/exercises/exercises.vue'
import ExerciseDetails from './views/exercises/exercise-details.vue'
import Routines from './views/routines/routines.vue'
import RoutineDetails from './views/routines/routine-details.vue'
import RoutineDetailsGrouped from './views/routines/routine-details-grouped.vue'
import RepCalculator from './views/rep-calculator.vue'

Vue.use(VueRouter)

function load (component) {
  // '@' is aliased to src/components
  return () => import(`@/${component}.vue`)
}

var router = new VueRouter({
  /*
   * NOTE! VueRouter "history" mode DOESN'T works for Cordova builds,
   * it is only to be used only for websites.
   *
   * If you decide to go with "history" mode, please also open /config/index.js
   * and set "build.publicPath" to something other than an empty string.
   * Example: '/' instead of current ''
   *
   * If switching back to default "hash" mode, don't forget to set the
   * build publicPath back to '' so Cordova builds work again.
   */

  routes: [
    { path: '/',
      component: Layout,
      children: [
        {
            path: '/',
            name: 'home',
            component: Home,
            meta: { title: 'diary' }
        },
        {
            path: '/foods',
            name: 'foods',
            component: Foods,
            meta: { title: 'foods' }
          },
          {
            path: '/foods/:id',
            name: 'food-details',
            component: FoodDetails,
            meta: { title: 'foodDetails' }
        },
          
        {
          path: '/recipes',
          name: 'recipes',
          component: Recipes,
          meta: { title: 'recipes' }
        },
        {
          path: '/recipes/:id',
          name: 'recipe-details',
          component: RecipeDetails,
          meta: { title: 'recipeDetails' }
        },
        {
          path: '/meal-rhythm',
          name: 'meal-rhythm',
          component: MealRhythm,
          meta: { title: 'mealRhythm' }
        },

        {
          path: '/login/:refreshToken?/:accessToken?',
          name: 'login',
          component: Login,
          meta: { title: 'login' }
        },
        {
          path: '/register',
          name: 'register',
          component: load('Register'),
          meta: { title: 'register' }
        },

        /*
        
        {
          path: '/nutrition-goals',
          name: 'nutrition-goals',
          component: load('NutritionGoals'),
          meta: { title: 'nutritionGoals' }
        },

        */
        {
          path: '/workouts',
          name: 'workouts',
          component: Workouts,
          meta: { title: 'workouts' }
        },
        {
          path: '/workouts/:id',
          name: 'workout-details',
          component: WorkoutDetailsGrouped,
          meta: { title: 'workoutDetails' }
        },
        {
          path: '/exercises',
          name: 'exercises',
          component: Exercises,
          meta: { title: 'exercises' }
        },
        {
          path: '/exercises/:id',
          name: 'exercise-details',
          component: ExerciseDetails,
          meta: { title: 'exerciseDetails' }
        },
        
        {
          path: '/routines',
          name: 'routines',
          component: Routines,
          meta: { title: 'routines' }
        },
        {
          path: '/routines/:id',
          name: 'routine-details',
          component: RoutineDetailsGrouped,
          meta: { title: 'routineDetails' }
        },
        {
          path: '/rep-calculator',
          name: 'rep-calculator',
          component: RepCalculator,
          meta: { title: 'repCalculator' }
        },
        /*
        {
          path: '/measurements',
          name: 'measurements',
          component: load('Measurements'),
          meta: { title: 'measurements' }
        }
        */
        ]
    },

    // Always leave this last one
    { path: '*', component: load('Error404') } // Not found
  ]
})
router.replace({name: 'login'});
export default router;