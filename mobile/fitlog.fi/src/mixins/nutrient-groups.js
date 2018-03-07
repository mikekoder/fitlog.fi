import constants from '../store/constants'

export default {
    computed: {
        $nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups;
        }
    }
}