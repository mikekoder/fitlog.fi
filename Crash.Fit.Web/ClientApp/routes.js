import Home from './views/home'
import Meals from './views/meals'
import Foods from './views/foods'
import Nutrients from './views/nutrients'
import Workouts from './views/workouts'
import Exercises from './views/exercises'
import Routines from './views/routines'
import NotFound from './views/notfound'

const main = [
    {
        path: '/',
        name: 'home',
        component: Home
    },
    {
        path: '/meals',
        name: 'meals',
        component: Meals
    },
    {
        path: '/foods',
        name: 'foods',
        component: Foods
    },
    {
        path: '/nutrients',
        name: 'nutrients',
        component: Nutrients
    },
    {
        path: '/workouts',
        name: 'workouts',
        component: Workouts
    },
    {
        path: '/exercises',
        name: 'exercises',
        component: Exercises
    },
    {
        path: '/routines',
        name: 'routines',
        component: Routines
    },
    {
        path: '*',
        name: 'notfound',
        component: NotFound
    }
]

export default main
