import { LocalStorage } from 'quasar'

export default {
    getItem(key) {
        return LocalStorage.get.item(key);
    },
    setItem(key, value) {
        LocalStorage.set(key, value);
    },
    removeItem(key) {
        LocalStorage.remove(key);
    }
}