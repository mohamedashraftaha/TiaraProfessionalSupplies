import { toast } from "vue3-toastify";

const showErrorToast = (message: string) => {
  toast.error(message, {
    position: "top-center",
    autoClose: 10000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    progress: undefined,
  })
}

const showSuccessToast = (message: string) => {
  toast.success(message, {
    position: "top-center",
    autoClose: 10000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    progress: undefined,
  })
}

const addToCartToast = (message: string) => {
  toast.success(message, {
    position: "bottom-right",
    autoClose: 10000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    progress: undefined,
  })
}

const showInfoToast = (message: string) => {
  toast.info(message, {
    position: "top-center",
    autoClose: 10000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    progress: undefined,
  })
}

const categoriesConst = [
  {
    id: 0, name: 'All Categories', icon: 'https://img.icons8.com/?size=100&id=42012&format=png&color=000000'
  },

  {
    id: 1,
    name: 'Premier Dental Products',
    icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/premierProducts.ico'
  },
  {
    id: 2,
    name: 'Avalon Biomed',
    icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/avalonbiomed.ico'
  },
  {
    id: 3,
    name: 'NuSmile',
    icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/nusmile.ico'
  },
  //{
  //  id: 20,
  //  name: 'Inibsa',
  //  icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/inibsa.ico'
  //},
  //{
  //  id: 21,
  //  name: 'Buzzy Pain Relief Device',
  //  icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/buzzy+(1).ico'
  //},
  //{
  //  id: 22,
  //  name: 'Medical Disposables (PPE)',
  //  icon: 'https://img.icons8.com/?size=100&id=14871&format=png&color=000000',
  //},
  //{
  //  id: 23,
  //  name: 'Dental Stickers',
  //  icon: 'https://img.icons8.com/?size=100&id=nGVdFaHhxo83&format=png&color=000000'
  //},
  //{
  //  id: 24,
  //  name: 'Meta Biomed',
  //  icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/meta_biomed.ico'
  //},
  //{
  //  id: 25,
  //  name: 'Digimed',
  //  icon: 'https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/digimed.ico'
  //},
  //{
  //  id: 8,
  //  name: 'Dental Gifts for Kids',
  //  icon: 'https://img.icons8.com/?size=100&id=3hPHKsDwKY9Q&format=png&color=000000'
  //},
  //{
  //  id: 26,
  //  name: 'Other Dental Product',
  //  icon: 'https://img.icons8.com/?size=100&id=14868&format=png&color=000000'
  //},
]


export {
  showErrorToast,
  showSuccessToast,
  showInfoToast,
  categoriesConst,
  addToCartToast
};
