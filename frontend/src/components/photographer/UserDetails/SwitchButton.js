import React, { useContext } from 'react'
import BootstrapSwitchButton from 'bootstrap-switch-button-react'
import lang from '../../../language/SWE/UserDetails.json'
import GlobalContext from '../../../context/GlobalContext'

const SwitchButton = ({ isChecked, setState, setNewChanges }) => {
  const context = useContext(GlobalContext)
  return (
    <div>
      <BootstrapSwitchButton
        style={{ width: 200, marginTop: -3, marginBottom: 7 }}
        onlabel={lang.btnactive}
        offlabel={lang.btninactive}
        onstyle='outline-primary'
        offstyle='outline-secondary'
        width={80}
        checked={context.userGlobal.available}
        onChange={checked => {
          setState(checked, setNewChanges(true))
        }}
      />
    </div>
  )
}

export default SwitchButton
