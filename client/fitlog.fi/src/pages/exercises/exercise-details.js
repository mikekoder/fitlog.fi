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
            targets: [],
            secondaryTargets: [],
            equipments: [],

            muscleGroups: [],
            equipmentOptions: []
        }
    },
    computed: {
        canSave(){
            return this.name && true;
        }
    },
    methods: {
        save() {
            var exercise = {
                id: this.id,
                name: this.name,
                percentageBW: this.percentageBW,
                targets: this.targets,
                secondaryTargets: this.secondaryTargets,
                equipments: this.equipments
            };

            this.$store.dispatch(constants.SAVE_EXERCISE, {
                exercise
            }).then(_ => {
                this.$router.replace({ name: 'exercises' });
            }).catch(_ => {});
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteExercise() {
            this.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise: { id: this.id }
            }).then(_ => {
                this.$router.push({ name: 'exercises' });
            }).catch(_ => {

            });
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
        var id = this.$route.params.id;
        Promise.all([
            this.$store.dispatch(constants.FETCH_MUSCLEGROUPS, { }),
            this.$store.dispatch(constants.FETCH_EQUIPMENT, {})]).then(values => {
                var muscleGroups = values[0];
                var equipments = values[1];
                this.muscleGroups = muscleGroups.map(mg => {return {...mg, label: mg.name, value: mg.id }});
                this.equipmentOptions = equipments.map(e => {return {...e, label: e.name, value: e.id }});

                if (id == constants.NEW_ID) {
                    this.id = undefined;
                    this.name = undefined;
                    this.targets = [];
                    this.secondaryTargets = [];
                    this.equipments = [];

                    this.$store.commit(constants.LOADING_DONE);
                }
                else {
                    this.$store.dispatch(constants.FETCH_EXERCISE, {
                        id
                    }).then(exercise => {
                        this.id = exercise.id;
                        this.name = exercise.name;
                        this.percentageBW = exercise.percentageBW;
                        this.targets = exercise.targets || [];
                        this.secondaryTargets = exercise.secondaryTargets || [];
                        this.equipments = exercise.equipments || [];
  
                        this.$store.commit(constants.LOADING_DONE);
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
