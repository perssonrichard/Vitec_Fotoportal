import React from 'react'
import { DropdownButton, Dropdown } from 'react-bootstrap'

const DropDownPhotographerList = ({ photographers, setChosenPhotographer }) => {
  return (
    <DropdownButton
      bsstyle='default'
      bssize='small'
      title='Välj fotograf'
      key={1}
      id='photographer-dropdown'
    >
      {photographers.map((p, key) => {
        if (p.available) {
          return (
            <div key={key}><Dropdown.Item className='broker-dropdown-item' onClick={() => { clickMenuItem(p) }}> {p.company} </Dropdown.Item>
            </div>
          )
        }
      }
      )}

    </DropdownButton>
  )

  function clickMenuItem (photographer) {
    setChosenPhotographer(photographer)
  }
}

export default DropDownPhotographerList
