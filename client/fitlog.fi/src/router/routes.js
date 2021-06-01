export default [
  {
    path: '/',
    component: () => import('layouts/default'),
    children: [
      {
        path: '/',
        name: 'home',
        component: () => import('pages/home/home.vue')
      },
      
      {
          path: '/foods',
          name: 'foods',
          component: () => import('pages/foods/foods.vue')
        },
        {
          path: '/foods/:id',
          name: 'food-details',
          component: () => import('pages/foods/food-details.vue')
      },       
      {
        path: '/recipes',
        name: 'recipes',
        component: () => import('pages/recipes/recipes.vue')
      },
      {
        path: '/recipes/:id',
        name: 'recipe-details',
        component: () => import('pages/recipes/recipe-details.vue')
      },
      {
        path: '/meal-rhythm',
        name: 'meal-rhythm',
        component: () => import('pages/meal-rhythm/meal-rhythm.vue')
      },   
      {
        path: '/login',
        name: 'login',
        component: () => import('pages/login.vue')
      },   
      {
        path: '/login/:refreshToken/:accessToken',
        name: 'login-tokens',
        component: () => import('pages/login.vue')
      },
      {
        path: '/register',
        name: 'register',
        component: () => import('pages/register')
      },
      {
        path: '/profile',
        name: 'profile',
        component: () => import('pages/profile/profile.vue')
      },
      {
        path: '/nutrition-goals',
        name: 'nutrition-goals',
        component: () => import('pages/nutrition-goals/nutrition-goals.vue')
      },
      {
        path: '/nutrition-goals/:id',
        name: 'nutrition-goal-details',
        component: () => import('pages/nutrition-goals/nutrition-goal-details.vue')
      },
      {
        path: '/meals',
        name: 'meals',
        component: () => import('pages/meals/meals.vue')
      },
      {
        path: '/workouts',
        name: 'workouts',
        component: () => import('pages/workouts/workouts.vue')
      },
      {
        path: '/workouts/:id',
        name: 'workout-details',
        component: () => import('pages/workouts/workout-details-grouped.vue')
      },
      {
        path: '/exercises',
        name: 'exercises',
        component: () => import('pages/exercises/exercises.vue')
      },
      {
        path: '/exercises/:id',
        name: 'exercise-details',
        component: () => import('pages/exercises/exercise-details.vue')
      },     
      {
        path: '/exercise-progress/:exerciseId',
        name: 'exercise-progress',
        component: () => import('pages/exercises/exercise-progress.vue')
      }, 
      {
        path: '/exercise-transfer/:exerciseId',
        name: 'exercise-transfer',
        component: () => import('pages/exercises/exercise-transfer.vue')
      }, 
      {
        path: '/routines',
        name: 'routines',
        component: () => import('pages/routines/routines.vue')
      },
      {
        path: '/routines/:id',
        name: 'routine-details',
        component: () => import('pages/routines/routine-details-grouped.vue')
      },
      {
        path: '/rep-calculator',
        name: 'rep-calculator',
        component: () => import('pages/rep-calculator.vue')
      },
      
      {
        path: '/measurements',
        name: 'measurements',
        component: () => import('pages/measurements/measurements.vue')
      },
      {
        path: '/measurement-details',
        name: 'measurement-details',
        component: () => import('pages/measurements/measurement-details.vue')
      },
      {
        path: '/measurement-progress/:measureId',
        name: 'measurement-progress',
        component: () => import('pages/measurements/measurement-progress.vue')
      },
      {
        path: '/improvements',
        name: 'improvements',
        component: () => import('pages/feedback/feedback.vue'), 
        meta: { type:'Improvement' }
      },
      {
        path: '/improvements/:id',
        name: 'improvement-details',
        component: () => import('pages/feedback/feedback-details.vue'), 
        meta: { type:'Improvement' }
      },
      {
        path: '/bugs',
        name: 'bugs',
        component: () => import('pages/feedback/feedback.vue'), 
        meta: { type:'Bug' }
      },
      {
        path: '/bugs/:id',
        name: 'bug-details',
        component: () => import('pages/feedback/feedback-details.vue'), 
        meta: { type:'Bug' }
      },
      {
        path: '/activitylevels',
        name: 'activity-levels',
        component: () => import('pages/activity-levels/activity-levels.vue')
      },
      {
        path: '/energy-expenditures',
        name: 'energy-expenditures',
        component: () => import('pages/energy-expenditures/energy-expenditures.vue')
      },
      {
        path: '/nutrition-chart',
        name: 'nutrition-chart',
        component: () => import('pages/chart/nutrition-chart.vue')
      },
      {
        path: '/food-comparison',
        name: 'food-comparison',
        component: () => import('pages/foods/food-comparison.vue')
      },
      {
        path: '/gdpr',
        name: 'gdpr',
        component: () => import('pages/gdpr.vue')
      }
    ]
  },
  { // Always leave this as last one
    path: '*',
    component: () => import('pages/404')
  }
]
