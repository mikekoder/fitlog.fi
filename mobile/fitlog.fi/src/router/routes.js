export default [
  {
    path: '/',
    component: () => import('layouts/default'),
    children: [
      /*
      { 
        path: '', 
        component: () => import('pages/index')
      },
      */
      {
        path: '/',
        name: 'home',
        component: () => import('pages/home/home.vue'),
        meta: { title: 'diary' }
      },
      
      {
          path: '/foods',
          name: 'foods',
          component: () => import('pages/foods/foods.vue'),
          meta: { title: 'foods' }
        },
        {
          path: '/foods/:id',
          name: 'food-details',
          component: () => import('pages/foods/food-details.vue'),
          meta: { title: 'foodDetails' }
      },       
      {
        path: '/recipes',
        name: 'recipes',
        component: () => import('pages/recipes/recipes.vue'),
        meta: { title: 'recipes' }
      },
      {
        path: '/recipes/:id',
        name: 'recipe-details',
        component: () => import('pages/recipes/recipe-details.vue'),
        meta: { title: 'recipeDetails' }
      },
      {
        path: '/meal-rhythm',
        name: 'meal-rhythm',
        component: () => import('pages/meal-rhythm/meal-rhythm.vue'), 
        meta: { title: 'mealRhythm' }
      },      
      {
        path: '/login/:refreshToken?/:accessToken?',
        name: 'login',
        component: () => import('pages/login.vue'),
        meta: { title: 'login' }
      },
      {
        path: '/register',
        name: 'register',
        component: () => import('pages/register'), 
        meta: { title: 'register' }
      },
      {
        path: '/profile',
        name: 'profile',
        component: () => import('pages/profile/profile.vue'), 
        meta: { title: 'profile' }
      },
      {
        path: '/nutrition-goals',
        name: 'nutrition-goals',
        component: () => import('pages/nutrition-goals/nutrition-goals.vue'), 
        meta: { title: 'nutritionGoals' }
      },
      {
        path: '/nutrition-goals/:id',
        name: 'nutrition-goal-details',
        component: () => import('pages/nutrition-goals/nutrition-goal-details.vue'), 
        meta: { title: 'nutritionGoals' }
      },
      {
        path: '/workouts',
        name: 'workouts',
        component: () => import('pages/workouts/workouts.vue'), 
        meta: { title: 'workouts' }
      },
      {
        path: '/workouts/:id',
        name: 'workout-details',
        component: () => import('pages/workouts/workout-details-grouped.vue'), 
        meta: { title: 'workoutDetails' }
      },
      {
        path: '/exercises',
        name: 'exercises',
        component: () => import('pages/exercises/exercises.vue'), 
        meta: { title: 'exercises' }
      },
      {
        path: '/exercises/:id',
        name: 'exercise-details',
        component: () => import('pages/exercises/exercise-details.vue'), 
        meta: { title: 'exerciseDetails' }
      },      
      {
        path: '/routines',
        name: 'routines',
        component: () => import('pages/routines/routines.vue'), 
        meta: { title: 'routines' }
      },
      {
        path: '/routines/:id',
        name: 'routine-details',
        component: () => import('pages/routines/routine-details-grouped.vue'), 
        meta: { title: 'routineDetails' }
      },
      {
        path: '/rep-calculator',
        name: 'rep-calculator',
        component: () => import('pages/rep-calculator.vue'), 
        meta: { title: 'repCalculator' }
      },
      
      {
        path: '/measurements',
        name: 'measurements',
        component: () => import('pages/measurements/measurements.vue'), 
        meta: { title: 'measurements' }
      },
      {
        path: '/measurement-details',
        name: 'measurement-details',
        component: () => import('pages/measurements/measurement-details.vue'), 
        meta: { title: 'measurements' }
      },

      {
        path: '/improvements',
        name: 'improvements',
        component: () => import('pages/feedback/feedback.vue'), 
        meta: { title: 'improvements', type:'Improvement' }
      },
      {
        path: '/improvements/:id',
        name: 'improvement-details',
        component: () => import('pages/feedback/feedback-details.vue'), 
        meta: { title: 'improvement', type:'Improvement' }
      },
      {
        path: '/bugs',
        name: 'bugs',
        component: () => import('pages/feedback/feedback.vue'), 
        meta: { title: 'bugs', type:'Bug' }
      },
      {
        path: '/bugs/:id',
        name: 'bug-details',
        component: () => import('pages/feedback/feedback-details.vue'), 
        meta: { title: 'bugDetails', type:'Bug' }
      },
      {
        path: '/activitylevels',
        name: 'activity-levels',
        component: () => import('pages/activity-levels/activity-levels.vue'), 
        meta: { title: 'activityLevels' }
      }
    ]
  },

  { // Always leave this as last one
    path: '*',
    component: () => import('pages/404')
  }
]
