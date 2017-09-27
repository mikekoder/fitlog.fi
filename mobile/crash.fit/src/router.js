import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

function load (component) {
  // '@' is aliased to src/components
  return () => import(`@/${component}.vue`)
}

export default new VueRouter({
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
      component: load('Layout'),
      children: [
        {
          path: '/login/:refreshToken?/:accessToken?',
          name: 'login',
          component: load('Login'),
          meta: { title: 'login' }
        },
        {
          path: '/register',
          name: 'register',
          component: load('Register'),
          meta: { title: 'register' }
        },
        {
          path: '/meals',
          name: 'meals',
          component: load('Meals'),
          meta: { title: 'meals' }
        },
        {
          path: '/foods',
          name: 'foods',
          component: load('Foods'),
          meta: { title: 'foods' }
        },
        {
          path: '/foods/:id',
          name: 'food-details',
          component: load('FoodDetails'),
          meta: { title: 'foodDetails' }
        },
        {
          path: '/recipes',
          name: 'recipes',
          component: load('Recipes'),
          meta: { title: 'recipes' }
        },
        {
          path: '/recipes/:id',
          name: 'recipe-details',
          component: load('RecipeDetails'),
          meta: { title: 'recipeDetails' }
        },
        {
          path: '/meal-rhythm',
          name: 'meal-rhythm',
          component: load('MealRhythm'),
          meta: { title: 'mealRhythm' }
        },
        {
          path: '/nutrition-goals',
          name: 'nutrition-goals',
          component: load('NutritionGoals'),
          meta: { title: 'nutritionGoals' }
        },
        {
          path: '/workouts',
          name: 'workouts',
          component: load('Workouts'),
          meta: { title: 'workouts' }
        },
        {
          path: '/workouts/:id',
          name: 'workout-details',
          component: load('WorkoutDetails'),
          meta: { title: 'workoutDetails' }
        },
        {
          path: '/exercises',
          name: 'exercises',
          component: load('Exercises'),
          meta: { title: 'exercises' }
        },
        {
          path: '/exercises/:id',
          name: 'exercise-details',
          component: load('ExerciseDetails'),
          meta: { title: 'exerciseDetails' }
        },
        {
          path: '/routines',
          name: 'routines',
          component: load('Routines'),
          meta: { title: 'routines' }
        },
        {
          path: '/routines/:id',
          name: 'routine-details',
          component: load('RoutineDetails'),
          meta: { title: 'routineDetails' }
        },
        {
          path: '/rep-calculator',
          name: 'rep-calculator',
          component: load('RepCalculator'),
          meta: { title: 'repCalculator' }
        },
        {
          path: '/measurements',
          name: 'measurements',
          component: load('Measurements'),
          meta: { title: 'measurements' }
        }]
    },

    // Always leave this last one
    { path: '*', component: load('Error404') } // Not found
  ]
})
