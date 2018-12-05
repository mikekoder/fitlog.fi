export default {
    data(){
        return {
            visible: false
        }
    },
    methods:{
        open(){
            this.visible = true;
        },
        close(){
            this.visible = false;
        }
    }
}