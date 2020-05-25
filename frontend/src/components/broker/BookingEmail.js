import lang from '../../language/SWE/BrokerDetails.json'

const BookingEmail = (order, photographer, broker) => {
  return `${lang.sendemail.hello} ${photographer.firstName} ${lang.sendemail.at} ${photographer.company}!\
          ${broker.company} ${lang.sendemail.hasbookedyouforphoto}\
            ${order.address} ${lang.sendemail.in} ${order.city}.\
            ${broker.name} ${lang.sendemail.hasleftmessage} ${order.description}.\
            ${lang.sendemail.automessage}\
            ${lang.sendemail.thanks} ${broker.company}\
            ${broker.name}, ${broker.cellphoneNr}
            `
}

export default BookingEmail
