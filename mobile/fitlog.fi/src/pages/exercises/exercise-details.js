import constants from '../../store/constants'
import utils from '../../utils'
import Help from './exercise-help'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components: {
        'exercise-help': Help
    },
    data() {
        return {
            id: null,
            name: null,
            percentageBW: null,
            targets: []
        }
    },
    computed: {
        muscleGroups() {
            return this.$store.state.training.muscleGroups;
        },
        canSave(){
            return this.name && true;
        }
    },
    methods: {
        save() {
            var self = this;

            var exercise = {
                id: self.id,
                name: self.name,
                percentageBW: self.percentageBW,
                targets: self.targets
            };

            self.$store.dispatch(constants.SAVE_EXERCISE, {
                exercise
            }).then(_ => {
                self.$router.replace({ name: 'exercises' });
            }).catch(_ => {});
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteExercise() {
            var self = this;
            self.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise: { id: self.id }
            }).then(_ => {
                self.$router.push({ name: 'exercises' });
            }).catch(_ => {

            });
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
        var self = this;
        var id = this.$route.params.id;
        self.$store.dispatch(constants.FETCH_MUSCLEGROUPS, { }).then(muscleGroups => {
            if (id == constants.NEW_ID) {
                self.id = undefined;
                self.name = undefined;
                self.targets = [];
                self.$store.commit(constants.LOADING_DONE);
            }
            else {
                self.$store.dispatch(constants.FETCH_EXERCISE, {
                    id
                }).then(exercise => {
                    self.id = exercise.id;
                    self.name = exercise.name;
                    self.percentageBW = exercise.percentageBW;
                    self.targets = exercise.targets;
                    self.$store.commit(constants.LOADING_DONE);
                });
            }
        });
    },
    mounted(){
        if(!this.name){
            this.$refs.nameInput.focus();
        }
    }
}
